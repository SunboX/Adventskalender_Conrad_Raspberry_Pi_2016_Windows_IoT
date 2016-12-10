using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Automation.Provider;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using TouchPanels.Devices;

namespace Tag7
{
    public class TouchHelper
    {
        private const string CalibrationFilename = "TSC2046";
        private Tsc2046 _tsc2046;
        private TouchPanels.TouchProcessor _processor;
        private Point _lastPosition = new Point(double.NaN, double.NaN);
        private IScrollProvider _currentScrollItem;
        private bool _isCalibrating; // Flag used to ignore the touch processor while calibrating
        private Page _page;

        public async Task Initialize(Page page)
        {
            _page = page;
            _tsc2046 = await Tsc2046.GetDefaultAsync();
            try
            {
                await _tsc2046.LoadCalibrationAsync(CalibrationFilename);
            }
            catch (System.IO.FileNotFoundException)
            {
                await CalibrateTouch(); // Initiate calibration if we don't have a calibration on file
            }
            catch (UnauthorizedAccessException)
            {
                // No access to documents folder
                await new Windows.UI.Popups.MessageDialog("Make sure the application manifest specifies access to the documents folder and declares the file type association for the calibration file.", "Configuration Error").ShowAsync();
                throw;
            }
            // Load up the touch processor and listen for touch events
            _processor = new TouchPanels.TouchProcessor(_tsc2046);
            _processor.PointerDown += Processor_PointerDown;
            _processor.PointerMoved += Processor_PointerMoved;
            _processor.PointerUp += Processor_PointerUp;
        }

        public void Teardown(Page mainPage)
        {
            if (_processor == null)
            {
                return;
            }
            // Unhooking from all the touch events, will automatically shut down the processor.
            // Remember to do this, or you view could be staying in memory.
            _processor.PointerDown -= Processor_PointerDown;
            _processor.PointerMoved -= Processor_PointerMoved;
            _processor.PointerUp -= Processor_PointerUp;
        }

        private void Processor_PointerDown(object sender, TouchPanels.PointerEventArgs e)
        {
            _currentScrollItem = FindElementsToInvoke(e.Position);
            _lastPosition = e.Position;
        }

        private void Processor_PointerMoved(object sender, TouchPanels.PointerEventArgs e)
        {
            if (_currentScrollItem != null)
            {
                var dx = e.Position.X - _lastPosition.X;
                var dy = e.Position.Y - _lastPosition.Y;
                if (!_currentScrollItem.HorizontallyScrollable) dx = 0;
                if (!_currentScrollItem.VerticallyScrollable) dy = 0;

                var h = Windows.UI.Xaml.Automation.ScrollAmount.NoAmount;
                var v = Windows.UI.Xaml.Automation.ScrollAmount.NoAmount;
                if (dx < 0) h = Windows.UI.Xaml.Automation.ScrollAmount.SmallIncrement;
                else if (dx > 0) h = Windows.UI.Xaml.Automation.ScrollAmount.SmallDecrement;
                if (dy < 0) v = Windows.UI.Xaml.Automation.ScrollAmount.SmallIncrement;
                else if (dy > 0) v = Windows.UI.Xaml.Automation.ScrollAmount.SmallDecrement;
                _currentScrollItem.Scroll(h, v);
            }
            _lastPosition = e.Position;
        }

        private void Processor_PointerUp(object sender, TouchPanels.PointerEventArgs e)
        {
            _currentScrollItem = null;
        }

        private async Task CalibrateTouch()
        {
            _isCalibrating = true;
            var calibration = await TouchPanels.UI.LcdCalibrationView.CalibrateScreenAsync(_tsc2046);
            _isCalibrating = false;
            _tsc2046.SetCalibration(calibration.A, calibration.B, calibration.C, calibration.D, calibration.E, calibration.F);
            try
            {
                await _tsc2046.SaveCalibrationAsync(CalibrationFilename);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        
        private IScrollProvider FindElementsToInvoke(Point screenPosition)
        {
            if (_isCalibrating)
            {
                return null;
            }

            var elements = VisualTreeHelper.FindElementsInHostCoordinates(new Point(screenPosition.X, screenPosition.Y), _page, false);
            // Search for buttons in the visual tree that we can invoke
            // If we can find an element button that implements the 'Invoke' automation pattern (usually buttons), we'll invoke it
            foreach (var e in elements.OfType<FrameworkElement>())
            {
                var element = e;
                object pattern = null;
                while (true)
                {
                    var peer = FrameworkElementAutomationPeer.FromElement(element);
                    if (peer != null)
                    {
                        pattern = peer.GetPattern(PatternInterface.Invoke);
                        if (pattern != null)
                        {
                            break;
                        }
                        pattern = peer.GetPattern(PatternInterface.Scroll);
                        if (pattern != null)
                        {
                            break;
                        }
                    }
                    var parent = VisualTreeHelper.GetParent(element);
                    if (parent is FrameworkElement)
                        element = parent as FrameworkElement;
                    else
                        break;
                }
                if (pattern == null)
                {
                    continue;
                }
                var p = pattern as IInvokeProvider;
                p?.Invoke();
                return pattern as IScrollProvider;
            }
            return null;
        }
    }
}
