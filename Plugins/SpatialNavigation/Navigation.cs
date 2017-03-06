using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Windows;
using System.Windows.Automation;
using UIAssistant.Utility.Extensions;

namespace UIAssistant.Plugin.SpatialNavigation
{
    class Navigation
    {
        #region EnumTargets
        public static HashSet<ControlType> _enumerateTargets;
        public static readonly HashSet<ControlType> _itemTargets = new HashSet<ControlType>(
            new ControlType[] {
                ControlType.Button,
                ControlType.Calendar,
                ControlType.CheckBox,
                ControlType.ComboBox,
                ControlType.Custom,
                ControlType.DataGrid,
                ControlType.DataItem,
                ControlType.Edit,
                ControlType.Hyperlink,
                ControlType.Image,
                ControlType.ListItem,
                // Panes often cause variety errors when SetFocus() is called
                // Issue: scintilla is a Pane...
                //ControlType.Pane,
                ControlType.RadioButton,
                ControlType.Slider,
                ControlType.Spinner,
                ControlType.SplitButton,
                //ControlType.Tab,
                ControlType.TabItem,
                ControlType.Thumb,
                ControlType.ToolBar,
                ControlType.TreeItem,
            });

        public static readonly HashSet<ControlType> _groupTargets = new HashSet<ControlType>(
            new ControlType[] {
                ControlType.Button,
                ControlType.Calendar,
                ControlType.CheckBox,
                ControlType.ComboBox,
                ControlType.Custom,
                ControlType.DataItem,
                ControlType.Edit,
                ControlType.Hyperlink,
                ControlType.Image,
                ControlType.List,
                // Panes often cause variety errors when SetFocus() is called
                // Issue: scintilla is a Pane...
                //ControlType.Pane,
                ControlType.RadioButton,
                ControlType.Slider,
                ControlType.Spinner,
                ControlType.SplitButton,
                ControlType.Tab,
                ControlType.Thumb,
                ControlType.ToolBar,
                ControlType.Tree,
            });

        public static readonly HashSet<ControlType> _ignoreElements = new HashSet<ControlType>(
            new ControlType[] {
                ControlType.MenuBar,
            });
        #endregion

        static object _lock = new object();
        static bool _isRunning = false;
        static List<Rect> _excludes = new List<Rect>();
        static double _weightingValue = 0d;

        public static void MoveTo(Direction direction, Unit unit)
        {
#if _STOPWATCH
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
#endif
            if (_isRunning)
            {
                return;
            }
            var t = Task.Run(() =>
            {
                lock (_lock)
                {
                    _isRunning = true;
                    try
                    {
                        InternalMoveTo(direction, unit);
                    }
                    finally
                    {
                        _isRunning = false;
                    }
                }
            });
            t.Wait();
#if _STOPWATCH
            sw.Stop();
            System.Diagnostics.Debug.Print($"{sw.ElapsedMilliseconds}ms");
#endif
        }

        public static void Initialize()
        {
            _excludes.Clear();
        }

        private static void InternalMoveTo(Direction direction, Unit unit)
        {
            Initialize();
            Task.Run(() => SpatialNavigation.UIAssistantAPI.SwitchTheme("General"));
            if (unit == Unit.Group)
            {
                _enumerateTargets = _groupTargets;
            }
            else
            {
                _enumerateTargets = _itemTargets;
            }

            var activeWindow = SpatialNavigation.UIAssistantAPI.ActiveWindow;
            var rootElement = activeWindow.Element;
            var current = AutomationElement.FocusedElement;
            if (current == null || current == rootElement)
            {
                MoveToFirst(rootElement);
                return;
            }

            _weightingValue = activeWindow.Bounds.BottomRight.GetDistance(new Point(0, 0));

            var currentInfo = current.Current;
            var parent = current.GetParent();

            if (parent != rootElement && unit == Unit.Group)
            {
                current = parent;
                currentInfo = parent.Current;
            }

            var focusedBounds = currentInfo.BoundingRectangle;

            _excludes.Add(focusedBounds);
            var type = currentInfo.ControlType;
            var function = new DirectionFunction(direction);

            if (parent != rootElement && unit != Unit.Group)
            {
                if (!TryMoveToSibling(parent, function, focusedBounds))
                {
                    TryMove(rootElement, function, focusedBounds, unit);
                    return;
                }
            }
            else
            {
                TryMove(rootElement, function, focusedBounds, unit);
            }
        }

        private static void MoveToFirst(AutomationElement root)
        {
            var targetControlTypes = _enumerateTargets.Select(x => new PropertyCondition(AutomationElement.ControlTypeProperty, x)).ToArray();
            var controlCondition = new OrCondition(targetControlTypes);
            var condition = new AndCondition(
                new PropertyCondition(AutomationElement.IsOffscreenProperty, false),
                new PropertyCondition(AutomationElement.IsKeyboardFocusableProperty, true),
                new PropertyCondition(AutomationElement.IsEnabledProperty, true),
                controlCondition);
            var candidate = root?.FindFirst(TreeScope.Descendants, condition);
            candidate?.SetFocus();
        }

        private static bool TryMoveToSibling(AutomationElement current, DirectionFunction function, Rect focusedBounds)
        {
            if (current == null)
            {
                return false;
            }

            var condition = new AndCondition(new PropertyCondition(AutomationElement.IsEnabledProperty, true), new PropertyCondition(AutomationElement.IsOffscreenProperty, false));
            _excludes.Add(current.Current.BoundingRectangle);
            var results = FindCandidatesInGroup(function, focusedBounds, current, condition);
            var destination = results.OrderBy(x => x.Distance).FirstOrDefault()?.Element;
            if (destination != null)
            {
                Move(destination, focusedBounds);
                return true;
            }
            return false;
        }

        private static bool TryMove(AutomationElement current, DirectionFunction function, Rect focusedBounds, Unit unit)
        {
            if (current == null)
            {
                return false;
            }

            var results = FindCandidates(function, focusedBounds, current, unit);
            var destination = results.OrderBy(x => x.Distance).ElementAtOrDefault(0)?.Element;
            if (destination != null)
            {
                if (destination.Current.ControlType == ControlType.TabItem)
                {
                    var tab = destination.GetParent();
                    var tabItems = tab.FindAll(TreeScope.Children, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.TabItem))?.Cast<AutomationElement>();
                    destination = tabItems?.FirstOrDefault(x => x.IsSelected()) ?? destination;
                }
                Move(destination, focusedBounds);
                return true;
            }
            return false;
        }

        private static void Move(AutomationElement destination, Rect sourceBounds)
        {
            var rect = destination.Current.BoundingRectangle;
            rect = new Rect(rect.Center(), new Size(1, 1));
            Task.Run(() =>
            {
                SpatialNavigation.UIAssistantAPI.TopMost = true;
                SpatialNavigation.UIAssistantAPI.ScaleIndicatorAnimation(sourceBounds, rect, false, 200, () => SpatialNavigation.UIAssistantAPI.TopMost = false);
            });
            destination.SetFocus();
        }

        static readonly Rect _zero = new Rect(0, 0, 0, 0);
        // Too complex...
        private static List<MeasurementResult> FindCandidates(DirectionFunction function, Rect focusedBounds, AutomationElement root, Unit unit)
        {
            List<MeasurementResult> results = new List<MeasurementResult>();
            var children = root.FindAll(TreeScope.Children, System.Windows.Automation.Condition.TrueCondition)?.Cast<AutomationElement>();
            if (children == null)
            {
                return results;
            }

            AutomationElement candidate = null;
            List<MeasurementResult> candidates = new List<MeasurementResult>();
            List<AutomationElement> focusedAncestors = new List<AutomationElement>();
            double nearestFocusableDistance = double.PositiveInfinity;

            foreach (var element in children)
            {
                var info = element.Current;
                var bounds = info.BoundingRectangle;
                var controlType = info.ControlType;

                // for WPF(Tab and TabItem)
                if (controlType == ControlType.Tab)
                {
                    focusedAncestors.Add(element);
                }
                else if (controlType == ControlType.TabItem)
                {
                    candidates.Add(new MeasurementResult(element, _weightingValue * 3));
                }

                if (_excludes.Any(x => x == bounds))
                {
                    continue;
                }
                if (bounds.Contains(focusedBounds) || bounds == _zero)
                {
                    focusedAncestors.Add(element);
                    continue;
                }
                if (!function.IsInDirection(bounds, focusedBounds) || bounds == focusedBounds)
                {
                    continue;
                }
                var distance = function.DistanceCaluculator(bounds, focusedBounds, _weightingValue);

                if (unit != Unit.Group || !controlType.IsContainer() || controlType == ControlType.Group)
                {
                    candidates.Add(new MeasurementResult(element, distance));
                }
                if (distance < nearestFocusableDistance)
                {
                    if (_enumerateTargets.Contains(controlType) && IsFocusable(info, controlType))
                    {
                        nearestFocusableDistance = distance;
                        candidate = element;
                    }
                }
            }

            focusedAncestors.ForEach(x => results.AddRange(FindCandidates(function, focusedBounds, x, unit)));
            candidates = candidates.OrderBy(x => x.Distance).ToList();
            foreach (var c in candidates)
            {
                var condition = new PropertyCondition(AutomationElement.IsEnabledProperty, true);
                var result = FindCandidatesInGroup(function, focusedBounds, c.Element, condition);
                if (result.Count > 0)
                {
                    results.AddRange(result);
                    break;
                }
            }
            if (candidate != null)
            {
                results.Add(new MeasurementResult(candidate, nearestFocusableDistance));
            }
            return results;
        }

        private static List<MeasurementResult> FindCandidatesInGroup(DirectionFunction function, Rect focusedBounds, AutomationElement group, System.Windows.Automation.Condition condition)
        {
            var results = new List<MeasurementResult>();
            var children = group.FindAll(TreeScope.Children, condition)?.Cast<AutomationElement>();
            if (children == null)
            {
                return results;
            }

            AutomationElement candidate = null;
            double nearestItemDistance = double.PositiveInfinity;
            foreach (var element in children)
            {
                var info = element.Current;
                var targetBounds = info.BoundingRectangle;
                var controlType = info.ControlType;

                if (controlType == ControlType.TreeItem)
                {
                    results.AddRange(FindCandidatesInGroup(function, focusedBounds, element, condition));
                }

                if (_ignoreElements.Contains(controlType) || targetBounds == focusedBounds || !function.IsInDirection(targetBounds, focusedBounds) || info.IsOffscreen)
                {
                    continue;
                }

                var distance = function.DistanceCaluculator(targetBounds, focusedBounds, _weightingValue);
                if (!IsFocusable(info, controlType) || targetBounds == _zero)
                {
                    distance = double.PositiveInfinity;
                }

                if (distance < nearestItemDistance && _enumerateTargets.Contains(controlType))
                {
                    nearestItemDistance = distance;
                    candidate = element;
                }
                else
                {
                    results.AddRange(FindCandidatesInGroup(function, focusedBounds, element, condition));
                }
            }

            if (candidate == null)
            {
                return results;
            }
            results.Add(new MeasurementResult(candidate, nearestItemDistance));
            return results;
        }

        // Some SelectionItem may be IsKeyboardFocusable = false but, in fact, it can be focused...
        private static bool IsFocusable(AutomationElement.AutomationElementInformation info, ControlType controlType)
        {
            return (info.IsKeyboardFocusable && info.IsEnabled) || controlType == ControlType.ListItem || controlType == ControlType.List;
        }
    }
}
