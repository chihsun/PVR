using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PVR
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.PreviewMouseLeftButtonDownEvent,
               new MouseButtonEventHandler(SelectivelyHandleMouseButton), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.MouseEnterEvent,
               new MouseEventHandler(SelectivelyHandleMouseEnter), true);
            EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);

            base.OnStartup(e);
        }

        private static void SelectivelyHandleMouseButton(object sender, MouseButtonEventArgs e)
        {
            var textbox = (sender as TextBox);
            if (textbox != null && !textbox.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }

        private static void SelectivelyHandleMouseEnter(object sender, MouseEventArgs e)
        {
            var textbox = (sender as TextBox);
            if (textbox != null && !textbox.IsKeyboardFocusWithin)
            {
                if (e.OriginalSource.GetType().Name == "TextBoxView")
                {
                    e.Handled = true;
                    textbox.Focus();
                }
            }
        }
        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
    }

    public partial class MainWindow
    {
        static public void TraverseVisualTree(Visual myMainWindow)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(myMainWindow);
            for (int i = 0; i < childrenCount; i++)
            {
                var visualChild = (Visual)VisualTreeHelper.GetChild(myMainWindow, i);
                if (visualChild is TextBox)
                {
                    TextBox tb = (TextBox)visualChild;
                    tb.Clear();
                }
                TraverseVisualTree(visualChild);
            }
        }
        private void TB_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }

        private void TB2_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.Decimal || e.Key == Key.OemPeriod || e.Key == Key.Subtract)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }
        private void TB3_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }
        private void TB4_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab || e.Key == Key.Oem2 || e.Key == Key.Divide)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }
        private void TB5_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Space)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            if (e.Key == Key.Down)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (e.Key == Key.Up)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
            }
        }
        private void ID_TextChanged(object sender, TextChangedEventArgs e)
        {
            Age.Text = string.Empty;
        }
        private void Age_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(Age.Text, out PVRData.Age))
            {

            }
            /*
            if (ID.Text.Length > 0 && Age.Text.Length >= 2)
                LoadData();
            else
                ClearData();
            */
            if (Age.Text.Length == 2)
            {
                if (int.TryParse(Age.Text, out int age) && age / 10.0 >= 1.5)
                    ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
            else if (Age.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            if (Age.Text.Length >= 2 && (RABI.Text.Length > 0 || LABI.Text.Length > 0))
            {
                RPVR.Focus();
            }
        }
        private void RPWV_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RPWV.Text, out PVRData.rpwv_r) && PVRData.rpwv_stiff)
                RPWV.Background = Brushes.LightPink;
            else
                RPWV.Background = SystemColors.WindowBrush;
            if (RPWV.Text.Length == 4)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next)); 
            }
        }

        private void LPWV_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(LPWV.Text, out PVRData.lpwv_r) && PVRData.lpwv_stiff)
                LPWV.Background = Brushes.LightPink;
            else
                LPWV.Background = SystemColors.WindowBrush;
            if (LPWV.Text.Length == 4)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void RPVR_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RPVR.Text, out PVRData.rpvr_r))
            {

            }
            if (RPVR.Text.Length == 2)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void LPVR_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(LPVR.Text, out PVRData.lpvr_r))
            {

            }
            if (LPVR.Text.Length == 2)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }
        private void RUT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(RUT.Text, out PVRData.rut))
            {

            }
            if (RUT.Text.Length == 3)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void LUT_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (int.TryParse(LUT.Text, out PVRData.lut))
            {

            }
            if (LUT.Text.Length == 3)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void RABI_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(RABI.Text, out PVRData.rabi_r) && PVRData.rabi_stenosis)
                RABI.Background = Brushes.LightPink;
            else
                RABI.Background = SystemColors.WindowBrush;
            if (RABI.Text.Length == 4)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void LABI_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (double.TryParse(LABI.Text, out PVRData.labi_r) && PVRData.labi_stenosis)
                LABI.Background = Brushes.LightPink;
            else
                LABI.Background = SystemColors.WindowBrush;
            if (LABI.Text.Length == 4)
            {
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void RSBP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RSBP.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void RDBP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RDBP.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void LSBP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LSBP.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void LDBP_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LDBP.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void RSBPA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RSBP_A.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void RDBPA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (RDBP_A.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void LSBPA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LSBP_A.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }

        private void LDBPA_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (LDBP_A.Text.Length == 3)
                ((UIElement)Keyboard.FocusedElement).MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
        }
        /*
        private void TB_TextChanged(Object sender, TextChangedEventArgs e)
        {
            VAL val = GetValalva();
            TB_VALR.Text = val.VALR == 0 ? String.Empty : Math.Round(val.VALR, 2, MidpointRounding.AwayFromZero).ToString();
            if (val.VALR != 0)
                TB_VAL.Text = TB_VALR.Text;
        }
        private void TB2_TextChanged(Object sender, TextChangedEventArgs e)
        {
            TB_BRS.Text = TB_BRR.Text;
        }
        private void TB3_TextChanged(Object sender, TextChangedEventArgs e)
        {
            VMRC vmr = GetVMR();
            TB_CO2VMRR.Text = vmr.VMR == 0 ? String.Empty : Math.Round(vmr.VMR, 2, MidpointRounding.AwayFromZero).ToString();
        }
        private void TB4_TextChanged(Object sender, TextChangedEventArgs e)
        {
            double sixa = Math.Round(SixAvergae(GetSixBreath()), 1, MidpointRounding.AwayFromZero);
            TB_6B.Text = sixa == 0 ? String.Empty : sixa.ToString();
        }
        private void TB5_TextChanged(Object sender, TextChangedEventArgs e)
        {
            CB_VALM.SelectedIndex = ValsalvaManeuer(GetVALMTPR());
        }
        */
    }
}
