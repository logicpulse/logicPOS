using Gtk;
using logicpos.Classes.Logic.Others;
using LogicPOS.UI.Alerts;
using LogicPOS.UI.Application;
using LogicPOS.UI.Application.Enums;
using LogicPOS.UI.Application.Screen;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Settings;
using LogicPOS.Utility;
using Serilog;
using System;
using System.Drawing;

namespace logicpos
{
    public class ExpressionEvaluatorExtended
    {
        //IN009257 Redimensionar botões para a resolução 1024 x 768
        public static Size sizePosBaseButtonSizeDefault;
        public static Size SizePosToolbarButtonSizeDefault { get; set; }
        public static Size SizePosTicketPadButtonSizeDefault { get; set; }
        public static Size SizePosTicketPadButtonDoubleWidthDefault { get; set; }
        public static Size SizePosToolbarButtonIconSizeDefault { get; set; }
        public static Size SizePosTicketPadButtonIconSizeDefault { get; set; }
        public static string FontDocumentsSizeDefault { get; set; }

        public static void ExpressionEvaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            // Size
            if (e.Name.Equals("Size") && e.Args.Count == 2)
            {
                // Get Expressions
                string widthExpression = e.EvaluateArg(0).ToString();
                string heightExpression = e.EvaluateArg(1).ToString();

                //if (heightExpression.Contains("startupWindowObjectsTablePadMarginLeftTop.Height + "))
                //{
                //    log.Debug("BREAK");
                //}

                // Get Results
                object widthResult = LogicPOSApp.ExpressionEvaluator.Evaluate(widthExpression);
                object heightResult = LogicPOSApp.ExpressionEvaluator.Evaluate(heightExpression);

                // Convert Results
                int width = Convert.ToInt32(e.EvaluateArg(0));
                int height = Convert.ToInt32(e.EvaluateArg(1));
                // Format Results
                e.Value = string.Format("{0},{1}", widthResult, heightResult);
            }
        }

        public static void InitVariablesStartupWindow()
        {
            // Put in Config and Load From Config
            try
            {
                // startupWindow.Objects.TablePadUser
                Size startupWindowObjectsTablePadUserMarginLeftTopSize = AppSettings.Instance.SizeStartupWindowObjectsTablePadUserMarginLeftTop;
                Size startupWindowObjectsTablePadUserButtonSize = AppSettings.Instance.SizeStartupWindowObjectsTablePadUserButton;
                Size startupWindowObjectsTablePadUserTablePadUserButtonPrevSize = AppSettings.Instance.SizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev;

                int startupWindowObjectsTablePadUserTableConfigRows = Convert.ToInt16(
                    (
                        AppSettings.Instance.AppScreenSize.Height
                        // Margin Top/Bottom
                        - (startupWindowObjectsTablePadUserMarginLeftTopSize.Height * 2)
                        - (startupWindowObjectsTablePadUserTablePadUserButtonPrevSize.Height * 2)
                        )
                        / startupWindowObjectsTablePadUserButtonSize.Height
                    );

                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserMarginLeftTopSize", startupWindowObjectsTablePadUserMarginLeftTopSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserButtonSize", startupWindowObjectsTablePadUserButtonSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTablePadUserButtonPrevSize", startupWindowObjectsTablePadUserTablePadUserButtonPrevSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTableConfigRows", startupWindowObjectsTablePadUserTableConfigRows);

                // startupWindow.Objects.NumberPadPin
                int startupWindowObjectsNumberPadPinRight = AppSettings.Instance.IntStartupWindowObjectsNumberPadPinRight;
                Size startupWindowObjectsNumberPadPinSize = AppSettings.Instance.SizeStartupWindowObjectsNumberPadPin;
                Size startupWindowObjectsNumberPadPinButtonSize = AppSettings.Instance.SizeStartupWindowObjectsNumberPadPinButton;

                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinRight", startupWindowObjectsNumberPadPinRight);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinSize", startupWindowObjectsNumberPadPinSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinButtonSize", startupWindowObjectsNumberPadPinButtonSize);

                // startupWindow.Objects.LabelVersion
                Size startupWindowObjectsLabelVersionSize = AppSettings.Instance.SizeStartupWindowObjectsLabelVersion;
                Size startupWindowObjectsLabelVersionSizeMarginRightBottomSize = AppSettings.Instance.SizeStartupWindowObjectsLabelVersionSizeMarginRightBottom;

                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSize", startupWindowObjectsLabelVersionSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSizeMarginRightBottomSize", startupWindowObjectsLabelVersionSizeMarginRightBottomSize);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
            }
        }

        public static void InitVariablesPosMainWindow()
        {
            // ConfigValues to ExpressionVars
            try
            {
                int intEventBoxPosTicketPadColumns = 4;
                int intEventBoxPosTicketPadRows = 4;

                if (AppSettings.Instance.OperationMode.IsDefaultMode())
                {
                    intEventBoxPosTicketPadRows = 5;
                }

                // Detect Dynamic Size
                bool appThemeCalcDynamicSize = AppSettings.Instance.AppThemeCalcDynamicSize;
                Size sizePosBaseButtonSize = new Size(0, 0);
                Size sizePosToolbarButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonDoubleWidth = new Size(0, 0);
                Size sizePosToolbarButtonIconSize = new Size(0, 0);
                Size sizePosTicketPadButtonIconSize = new Size(0, 0);
                string fontTicketListColumnTitle = string.Empty;
                string fontTicketListColumn = string.Empty;

                string enumScreenSize = string.Format("res{0}x{1}", AppSettings.Instance.AppScreenSize.Width, AppSettings.Instance.AppScreenSize.Height);

                try
                {
                    ScreenResolution screenSizeEnum = (ScreenResolution)Enum.Parse(typeof(ScreenResolution), enumScreenSize, true);

                    if (appThemeCalcDynamicSize)
                    {
                        switch (screenSizeEnum)
                        {
                            case ScreenResolution.res800x600:
                                sizePosBaseButtonSize = new Size(100, 75);
                                sizePosToolbarButtonSize = new Size(54, 38);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(22, 22);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 8";
                                fontTicketListColumn = "8";
                                FontDocumentsSizeDefault = "6";
                                break;
                            case ScreenResolution.res1024x600:
                            case ScreenResolution.res1024x768:
                                sizePosBaseButtonSize = new Size(120, 90);
                                sizePosToolbarButtonSize = new Size(80, 60);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(34, 34);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 9";
                                fontTicketListColumn = "9";
                                FontDocumentsSizeDefault = "6";
                                break;
                            case ScreenResolution.res1152x864:
                            case ScreenResolution.res1280x720:
                            case ScreenResolution.res1280x768:
                            case ScreenResolution.res1280x800:
                            case ScreenResolution.res1280x1024:
                            case ScreenResolution.res1360x768:
                            case ScreenResolution.res1366x768:
                            case ScreenResolution.res1440x900:
                            case ScreenResolution.res1536x864:
                            case ScreenResolution.res1600x900:
                                fontTicketListColumnTitle = "Bold 10";
                                fontTicketListColumn = "10";
                                sizePosBaseButtonSize = new Size(140, 105);
                                sizePosToolbarButtonSize = new Size(100, 75);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(42, 42);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                FontDocumentsSizeDefault = "8";
                                break;
                            case ScreenResolution.res1680x1050:
                            case ScreenResolution.res1920x1080:
                            case ScreenResolution.res1920x1200:
                            case ScreenResolution.res2160x1440:
                            case ScreenResolution.res2560x1080:
                            case ScreenResolution.res2560x1440:
                            case ScreenResolution.res3440x1440:
                            case ScreenResolution.res3840x2160:
                                sizePosBaseButtonSize = new Size(160, 120);
                                sizePosToolbarButtonSize = new Size(120, 90);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(50, 50);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 10";
                                fontTicketListColumn = "10";
                                FontDocumentsSizeDefault = "10";
                                break;
                            /* IN008023: apply "1024x768" settings as default */
                            default:
                                sizePosBaseButtonSize = new Size(120, 90);
                                sizePosToolbarButtonSize = new Size(80, 60);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(34, 34);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 9";
                                fontTicketListColumn = "9";
                                FontDocumentsSizeDefault = "6";
                                break;
                        }
                    }
                    else
                    {
                        // Use Defaults from Config
                        sizePosBaseButtonSize = AppSettings.Instance.SizePosBaseButton;
                        sizePosToolbarButtonSize = AppSettings.Instance.SizePosToolbarButton;
                        sizePosTicketPadButtonSize = AppSettings.Instance.SizePosTicketPadButton;
                        sizePosTicketPadButtonDoubleWidth = AppSettings.Instance.SizePosTicketPadButtonDoubleWidth;
                        sizePosToolbarButtonIconSize = AppSettings.Instance.SizePosToolbarButtonIcon;
                        sizePosTicketPadButtonIconSize = AppSettings.Instance.SizePosTicketPadButtonIcon;
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception");
                    var messageDialog = new CustomAlert(LoginWindow.Instance)
                                              .WithMessage($"Unsupported Resolution Detected: {AppSettings.Instance.AppScreenSize.Width}x{AppSettings.Instance.AppScreenSize.Height}")
                                              .WithMessageType(MessageType.Error)
                                              .WithButtonsType(ButtonsType.Close)
                                              .WithTitleResource(GeneralUtils.GetResourceByName("dialog_message_operation_successfully"))
                                              .ShowAlert();
                    //Utils.ShowMessageTouchUnsupportedResolutionDetectedAndExit(LoginWindow.Instance, AppSettings.Instance.AppScreenSize.Width, AppSettings.Instance.AppScreenSize.Height); :: dúvida na substituição ::LUCIANO
                }

                int posMainWindowComponentsMargin = AppSettings.Instance.IntPosMainWindowComponentsMargin;
                int posMainWindowEventBoxPosTicketListColumnWidth = sizePosTicketPadButtonSize.Width * 4;
                int posMainWindowEventBoxStatusBar1And2Height = AppSettings.Instance.IntPosMainWindowEventBoxStatusBar1And2Height;
                // Remove Margin, Column Price, Qnt, Total
                int posMainWindowEventBoxPosTicketListColumnsDesignationWidth = posMainWindowEventBoxPosTicketListColumnWidth - 10 - 65 - 55 - 75;

                //IN009257 Redimensionar botões para a resolução 1024 x 768.
                sizePosBaseButtonSizeDefault = sizePosBaseButtonSize;
                SizePosToolbarButtonSizeDefault = new Size((int)(sizePosToolbarButtonSize.Width / 1.4), (int)(sizePosToolbarButtonSize.Height / 1.4));
                SizePosTicketPadButtonSizeDefault = sizePosTicketPadButtonSize;
                SizePosTicketPadButtonDoubleWidthDefault = sizePosTicketPadButtonDoubleWidth;
                SizePosToolbarButtonIconSizeDefault = new Size((int)(sizePosToolbarButtonIconSize.Width / 1.7), (int)(sizePosToolbarButtonIconSize.Height / 1.7));
                SizePosTicketPadButtonIconSizeDefault = sizePosTicketPadButtonIconSize;
                //IN009257 END

                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadColumns", intEventBoxPosTicketPadColumns);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadRows", intEventBoxPosTicketPadRows);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonSize", sizePosTicketPadButtonSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonDoubleWidthSize", sizePosTicketPadButtonDoubleWidth);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowToolbarButtonSize", sizePosToolbarButtonSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowComponentsMargin", posMainWindowComponentsMargin);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnWidth", posMainWindowEventBoxPosTicketListColumnWidth);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxStatusBar1And2Height", posMainWindowEventBoxStatusBar1And2Height);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventboxToolbarIconSize", sizePosToolbarButtonIconSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketPadButtonsIconSize", sizePosTicketPadButtonIconSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnsDesignationWidth", posMainWindowEventBoxPosTicketListColumnsDesignationWidth);

                // Calculate TablePads TableConfig and Button Sizes, from starter sizePosBaseButton

                //Override this from Config
                int usefullAreaForMainTablePadsWidth = AppSettings.Instance.AppScreenSize.Width - posMainWindowComponentsMargin - (posMainWindowEventBoxPosTicketListColumnWidth + posMainWindowComponentsMargin * 2);
                int usefullAreaForMainTablePadsHeight = AppSettings.Instance.AppScreenSize.Height - (posMainWindowEventBoxStatusBar1And2Height * 2) - posMainWindowComponentsMargin - posMainWindowEventBoxStatusBar1And2Height - (sizePosToolbarButtonSize.Height + posMainWindowComponentsMargin * 2);
                // Get Columns and Rows based on sizePosBaseButton, leaving remains space to split after
                int guessedTablePadColumns = Convert.ToInt16(usefullAreaForMainTablePadsWidth / sizePosBaseButtonSize.Width);
                int guessedTablePadRows = Convert.ToInt16(usefullAreaForMainTablePadsHeight / sizePosBaseButtonSize.Height);
                // Get Remain Space
                int remainSpaceInColumns = usefullAreaForMainTablePadsWidth - (sizePosBaseButtonSize.Width * guessedTablePadColumns);
                int remainSpaceInRows = usefullAreaForMainTablePadsHeight - (sizePosBaseButtonSize.Height * guessedTablePadRows);
                // Distribute Remain Space for Calculated Buttons
                int remainSpaceForEveryButtonWidth = Convert.ToInt16(remainSpaceInColumns / guessedTablePadColumns);
                int remainSpaceForEveryButtonHeight = Convert.ToInt16(remainSpaceInRows / guessedTablePadRows);
                // Finished Add remainSpace to Button Size
                sizePosBaseButtonSize.Width += remainSpaceForEveryButtonWidth;
                sizePosBaseButtonSize.Height += remainSpaceForEveryButtonHeight;

                // Create Variables for TableConfig to Override Config Defaults
                TableConfig tableConfigTablePadFamily = new TableConfig(1, Convert.ToUInt16(guessedTablePadRows - 1));
                TableConfig tableConfigTablePadSubFamily = new TableConfig(Convert.ToUInt16(guessedTablePadColumns - 1), 1);
                TableConfig tableConfigTablePadArticle = new TableConfig(Convert.ToUInt16(guessedTablePadColumns - 1), Convert.ToUInt16(guessedTablePadRows - 1));

                // After has final sizePosBaseButton
                Point positionButtonFavorites = new Point(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + posMainWindowComponentsMargin);
                Point positionTablePadFamily = new Point(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + sizePosBaseButtonSize.Height + posMainWindowComponentsMargin);
                Point positionTablePadSubFamily = new Point(posMainWindowComponentsMargin + sizePosBaseButtonSize.Width, (posMainWindowEventBoxStatusBar1And2Height * 2) + posMainWindowComponentsMargin);
                Point positionTablePadArticle = new Point(posMainWindowComponentsMargin + sizePosBaseButtonSize.Width, (posMainWindowEventBoxStatusBar1And2Height * 2) + sizePosBaseButtonSize.Height + posMainWindowComponentsMargin);
                // ButtonNext/Prev
                Point tablePadFamilyButtonPrevPosition = new Point(posMainWindowComponentsMargin, posMainWindowEventBoxStatusBar1And2Height + posMainWindowComponentsMargin);
                Point tablePadFamilyButtonNextPosition = new Point(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + (posMainWindowComponentsMargin / 2) + (sizePosBaseButtonSize.Height * guessedTablePadRows) + (posMainWindowComponentsMargin / 2));
                Point tablePadSubFamilyButtonPrevPosition = new Point(posMainWindowComponentsMargin + (sizePosBaseButtonSize.Width * (guessedTablePadColumns - 1)), tablePadFamilyButtonPrevPosition.Y);
                Point tablePadSubFamilyButtonNextPosition = new Point(posMainWindowComponentsMargin + (sizePosBaseButtonSize.Width * (guessedTablePadColumns - 1)) + (sizePosBaseButtonSize.Width / 2), tablePadFamilyButtonPrevPosition.Y);
                Point tablePadArticleButtonPrevPosition = new Point(tablePadSubFamilyButtonPrevPosition.X, tablePadFamilyButtonNextPosition.Y);
                Point tablePadArticleButtonNextPosition = new Point(tablePadSubFamilyButtonNextPosition.X, tablePadFamilyButtonNextPosition.Y);
                Size sizePosSmallButtonScrollerSize = new Size(Convert.ToInt16(sizePosBaseButtonSize.Width / 2), posMainWindowEventBoxStatusBar1And2Height);

                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowBaseButtonSize", sizePosBaseButtonSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyTableConfig", tableConfigTablePadFamily);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyTableConfig", tableConfigTablePadSubFamily);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleTableConfig", tableConfigTablePadArticle);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowButtonFavoritesPosition", positionButtonFavorites);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyPosition", positionTablePadFamily);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyPosition", positionTablePadSubFamily);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticlePosition", positionTablePadArticle);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonPrevPosition", tablePadFamilyButtonPrevPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonNextPosition", tablePadFamilyButtonNextPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonPrevPosition", tablePadSubFamilyButtonPrevPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonNextPosition", tablePadSubFamilyButtonNextPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonPrevPosition", tablePadArticleButtonPrevPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonNextPosition", tablePadArticleButtonNextPosition);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowSmallButtonScrollerSize", sizePosSmallButtonScrollerSize);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnTitleFont", fontTicketListColumnTitle);
                LogicPOSApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnFont", fontTicketListColumn);

                // Test Expressions Here
                //string hardCodeExpression = "Size((tableConfigTablePadArticle.Columns - 2) * posMainWindowBaseButtonSize.Width, posMainWindowEventBoxStatusBar1And2Height)";
                //string hardCodeResult = GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString();
                //log.Debug(string.Format("result: [{0}]", GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString()));
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Exception");
            }
        }
    }
}
