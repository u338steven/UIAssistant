using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using UIAssistant.Utility.Extensions;
using UIAssistant.Utility.Win32;

namespace UIAssistant.Plugin.SearchByText.Items
{
    class MenuItemUIA : SearchByTextItem
    {
        public List<string> Ancestors { get; private set; }
        public AutomationElement Element { get; private set; }
        public AutomationElement Root { get; private set; }

        public MenuItemUIA(string name, string fullName, Rect bounds, bool isEnabled, bool canExpand = false, AutomationElement element = null, AutomationElement root = null, List<string> ancestors = null)
            : base(name, fullName, bounds, isEnabled, canExpand)
        {
            Element = element;
            Root = root;
            Ancestors = ancestors;
        }

        public override void Prepare()
        {
        }

        public override void Execute()
        {
            if (!IsEnabled)
            {
                return;
            }
            Prepare();

            if (Root.IsWPF())
            {
                Task.Run(() =>
                {
                    AutomationElement element = Root;
                    Ancestors.ForEach(x =>
                    {
                        element = GetCurrentElement(element, x);
                        element.TryExpand();
                    });
                    element = GetCurrentElement(element);
                    element.TryDoDefaultAction();
                });
                return;
            }

            Task.Run(() =>
            {
                if (Ancestors.Count > 0)
                {
                    var observer = SearchByText.UIAssistantAPI.GetObserver(Interfaces.Events.ObserberKinds.PopupObserver);
                    var shouldBeClosed = false;
                    observer.Callback += x =>
                    {
                        try
                        {
                            if (Ancestors.Count == 0)
                            {
                                shouldBeClosed = true;
                                observer.Dispose();
                                var element = GetCurrentElement(x);
                                if (element == null)
                                {
                                    SearchByText.UIAssistantAPI.NotifyWarnMessage(Consts.PluginName, "Cannot find the selected menu item");
                                    return;
                                }

                                // 見えない要素(要スクロール)の場合、実行できないので実行は諦める
                                if (element.Current.IsOffscreen)
                                {
                                    SearchByText.UIAssistantAPI.NotifyWarnMessage(Consts.PluginName, "Cannot run the selected menu item");
                                    return;
                                    //System.Diagnostics.Debug.Print($"1: {element.Current.AccessKey}, {element.Current.IsOffscreen}, {element.Current.IsEnabled}, {element.Current.BoundingRectangle}");
                                    //Core.Input.KeyboardOperation.SendKeys(System.Windows.Input.Key.Up);
                                    //System.Threading.Thread.Sleep(200);
                                    //Task.Run(() => element = GetCurrentElement(x)).Wait();
                                    //System.Diagnostics.Debug.Print($"1: {element.Current.AccessKey}, {element.Current.IsOffscreen}, {element.Current.IsEnabled}, {element.Current.BoundingRectangle}");
                                }

                                element.TryDoDefaultAction();
                                return;
                            }
                            var elements = GetCandidates(x, AutomationElement.NameProperty, Ancestors[0]);
                            Ancestors.RemoveAt(0);
                            foreach (var element in elements)
                            {
                                if (element.CanExpandElement(element.Current.ControlType))
                                {
                                    element.TryExpand();
                                    break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            SearchByText.UIAssistantAPI.NotifyErrorMessage(Consts.PluginName, ex.Message + "\nThe menu may not be scrolled");
                        }
                        finally
                        {
                            if (shouldBeClosed)
                            {
                                System.Threading.Thread.Sleep(200);
                                Win32Interop.SendMessage(Root.Current.NativeWindowHandle, Win32Interop.WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                            }
                        }
                    };
                    observer.Observe();

                    var propCondition = new PropertyCondition(AutomationElement.NameProperty, Ancestors[0]);
                    var candidate = Root.FindAll(TreeScope.Descendants, propCondition);
                    if (candidate.Count == 0)
                    {
                        SearchByText.UIAssistantAPI.NotifyWarnMessage(Consts.PluginName, "Cannot find the selected menu item");
                        return;
                    }
                    Ancestors.RemoveAt(0);
                    candidate[0].TryExpand();

                    observer.Wait();
                }
                else
                {
                    Element.DoAction();
                }
            });
        }
    }
}
