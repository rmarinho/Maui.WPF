using System.Diagnostics;
using System.Drawing;
using System.Windows.Automation;

namespace UITests.Helpers;

/// <summary>
/// UIAutomation helper for finding and interacting with WPF/WinUI controls.
/// </summary>
public static class AutomationHelper
{
    /// <summary>
    /// Get the automation root for a process's main window.
    /// </summary>
    public static AutomationElement GetRoot(Process proc)
    {
        proc.Refresh();
        if (proc.MainWindowHandle == IntPtr.Zero)
            throw new InvalidOperationException($"Process {proc.Id} has no main window");
        return AutomationElement.FromHandle(proc.MainWindowHandle);
    }

    /// <summary>
    /// Find all elements matching a control type.
    /// </summary>
    public static AutomationElementCollection FindAll(AutomationElement root, ControlType controlType)
    {
        return root.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.ControlTypeProperty, controlType));
    }

    /// <summary>
    /// Find elements by name (exact match).
    /// </summary>
    public static AutomationElementCollection FindByName(AutomationElement root, string name)
    {
        return root.FindAll(TreeScope.Descendants,
            new PropertyCondition(AutomationElement.NameProperty, name));
    }

    /// <summary>
    /// Find a flyout item by its text label and click it.
    /// Returns true if found and clicked.
    /// </summary>
    public static bool ClickFlyoutItem(AutomationElement root, string itemName)
    {
        // Flyout items are Text elements in the left panel; their parent is invokable
        var texts = FindByName(root, itemName);
        foreach (AutomationElement t in texts)
        {
            var rect = t.Current.BoundingRectangle;
            if (rect.IsEmpty || rect.Width <= 0) continue;

            // Walk up to find invokable parent
            var walker = TreeWalker.ControlViewWalker;
            var parent = walker.GetParent(t);
            while (parent != null && parent != root)
            {
                try
                {
                    var invoke = (InvokePattern)parent.GetCurrentPattern(InvokePattern.Pattern);
                    invoke.Invoke();
                    return true;
                }
                catch (InvalidOperationException) { }
                parent = walker.GetParent(parent);
            }

            // Fallback: click at the element's center
            ClickAt(rect);
            return true;
        }
        return false;
    }

    /// <summary>
    /// Select a value in a ComboBox by expanding it and choosing an item.
    /// </summary>
    public static bool SelectComboBoxItem(AutomationElement root, string itemName)
    {
        var combos = FindAll(root, ControlType.ComboBox);
        foreach (AutomationElement combo in combos)
        {
            try
            {
                var expand = (ExpandCollapsePattern)combo.GetCurrentPattern(ExpandCollapsePattern.Pattern);
                expand.Expand();
                Thread.Sleep(500);

                var items = FindAll(combo, ControlType.ListItem);
                foreach (AutomationElement item in items)
                {
                    if (item.Current.Name == itemName)
                    {
                        var sel = (SelectionItemPattern)item.GetCurrentPattern(SelectionItemPattern.Pattern);
                        sel.Select();
                        expand.Collapse();
                        return true;
                    }
                }
                expand.Collapse();
            }
            catch { }
        }
        return false;
    }

    /// <summary>
    /// Get all text element names in a bounding region (for verifying content).
    /// </summary>
    public static List<string> GetTextElementsInRegion(AutomationElement root, System.Windows.Rect region)
    {
        var result = new List<string>();
        var texts = FindAll(root, ControlType.Text);
        foreach (AutomationElement t in texts)
        {
            var r = t.Current.BoundingRectangle;
            if (r.IsEmpty) continue;
            if (r.Left >= region.Left && r.Right <= region.Right &&
                r.Top >= region.Top && r.Bottom <= region.Bottom)
            {
                if (!string.IsNullOrWhiteSpace(t.Current.Name))
                    result.Add(t.Current.Name);
            }
        }
        return result;
    }

    /// <summary>
    /// Click at a screen position using mouse_event.
    /// </summary>
    public static void ClickAt(System.Windows.Rect rect)
    {
        var centerX = (int)(rect.X + rect.Width / 2);
        var centerY = (int)(rect.Y + rect.Height / 2);
        System.Windows.Forms.Cursor.Position = new Point(centerX, centerY);
        Thread.Sleep(100);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);
        Thread.Sleep(50);
        NativeMethods.mouse_event(NativeMethods.MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }

    /// <summary>
    /// Wait for a specific text element to appear.
    /// </summary>
    public static bool WaitForText(AutomationElement root, string text, TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        while (DateTime.UtcNow < deadline)
        {
            var elements = FindByName(root, text);
            if (elements.Count > 0) return true;
            Thread.Sleep(500);
        }
        return false;
    }
}

internal static class NativeMethods
{
    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern void mouse_event(uint dwFlags, int dx, int dy, uint dwData, int dwExtraInfo);

    [System.Runtime.InteropServices.DllImport("user32.dll")]
    public static extern bool SetForegroundWindow(IntPtr hWnd);

    public const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    public const uint MOUSEEVENTF_LEFTUP = 0x0004;
}
