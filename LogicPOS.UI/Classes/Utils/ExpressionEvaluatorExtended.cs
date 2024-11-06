using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.Others;
using LogicPOS.Settings;
using LogicPOS.UI.Application;
using LogicPOS.UI.Components.Windows;
using LogicPOS.UI.Extensions;
using LogicPOS.Utility;
using System;
using System.Drawing;

namespace logicpos
{
    public class ExpressionEvaluatorExtended
    {
		//IN009257 Redimensionar botões para a resolução 1024 x 768
        public static Size sizePosBaseButtonSizeDefault;
        public static Size sizePosToolbarButtonSizeDefault { get; set; }
        public static Size sizePosTicketPadButtonSizeDefault;
        public static Size sizePosTicketPadButtonDoubleWidthDefault;
        public static Size sizePosToolbarButtonIconSizeDefault;
        public static Size sizePosTicketPadButtonIconSizeDefault;
        public static string fontDocumentsSizeDefault;

        public static void ExpressionEvaluator_EvaluateFunction(object sender, FunctionEvaluationEventArg e)
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
                object widthResult = LogicPOSAppContext.ExpressionEvaluator.Evaluate(widthExpression);
                object heightResult = LogicPOSAppContext.ExpressionEvaluator.Evaluate(heightExpression);

                // Convert Results
                int width = Convert.ToInt32(e.EvaluateArg(0));
                int height = Convert.ToInt32(e.EvaluateArg(1));
                // Format Results
                e.Value = string.Format("{0},{1}", widthResult, heightResult);
            }
        }

        public static void InitVariablesStartupWindow()
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

            // Put in Config and Load From Config
            try
            {
                // startupWindow.Objects.TablePadUser
                Size startupWindowObjectsTablePadUserMarginLeftTopSize = AppSettings.Instance.sizeStartupWindowObjectsTablePadUserMarginLeftTop;
                Size startupWindowObjectsTablePadUserButtonSize = AppSettings.Instance.sizeStartupWindowObjectsTablePadUserButton;
                Size startupWindowObjectsTablePadUserTablePadUserButtonPrevSize = AppSettings.Instance.sizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev;

                int startupWindowObjectsTablePadUserTableConfigRows = Convert.ToInt16(
                    (
                        LogicPOSAppContext.ScreenSize.Height
                        // Margin Top/Bottom
                        - (startupWindowObjectsTablePadUserMarginLeftTopSize.Height * 2)
                        - (startupWindowObjectsTablePadUserTablePadUserButtonPrevSize.Height * 2)
                        )
                        / startupWindowObjectsTablePadUserButtonSize.Height
                    );

                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserMarginLeftTopSize", startupWindowObjectsTablePadUserMarginLeftTopSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserButtonSize", startupWindowObjectsTablePadUserButtonSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTablePadUserButtonPrevSize", startupWindowObjectsTablePadUserTablePadUserButtonPrevSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTableConfigRows", startupWindowObjectsTablePadUserTableConfigRows);

                // startupWindow.Objects.NumberPadPin
                int startupWindowObjectsNumberPadPinRight = AppSettings.Instance.intStartupWindowObjectsNumberPadPinRight;
                Size startupWindowObjectsNumberPadPinSize = AppSettings.Instance.sizeStartupWindowObjectsNumberPadPin;
                Size startupWindowObjectsNumberPadPinButtonSize = AppSettings.Instance.sizeStartupWindowObjectsNumberPadPinButton;

                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinRight", startupWindowObjectsNumberPadPinRight);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinSize", startupWindowObjectsNumberPadPinSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinButtonSize", startupWindowObjectsNumberPadPinButtonSize);

                // startupWindow.Objects.LabelVersion
                Size startupWindowObjectsLabelVersionSize = AppSettings.Instance.sizeStartupWindowObjectsLabelVersion;
                Size startupWindowObjectsLabelVersionSizeMarginRightBottomSize = AppSettings.Instance.sizeStartupWindowObjectsLabelVersionSizeMarginRightBottom;

                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSize", startupWindowObjectsLabelVersionSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSizeMarginRightBottomSize", startupWindowObjectsLabelVersionSizeMarginRightBottomSize);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }

        public static void InitVariablesPosMainWindow()
        {
            //Log4Net
            log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


            // Put in Config and Load From Config

            // ConfigValues to ExpressionVars
            try
            {
                // Config
                /* IN008024 */
                /* For  CustomAppOperationMode.RETAIL */
				//IN009345 - Front-End - Falta de funcionalidades em modo Retalho (Botões) 
                int intEventBoxPosTicketPadColumns = 4;
                int intEventBoxPosTicketPadRows = 4;

                if (AppOperationModeSettings.IsDefaultTheme)
                {
                    //intEventBoxPosTicketPadColumns = 4;
                    intEventBoxPosTicketPadRows = 5;
                }

                // Detect Dynamic Size
                bool appThemeCalcDynamicSize = AppSettings.Instance.appThemeCalcDynamicSize;
                Size sizePosBaseButtonSize = new Size(0, 0);
                Size sizePosToolbarButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonDoubleWidth = new Size(0, 0);
                Size sizePosToolbarButtonIconSize = new Size(0, 0);
                Size sizePosTicketPadButtonIconSize = new Size(0, 0);
                string fontTicketListColumnTitle = string.Empty;
                string fontTicketListColumn = string.Empty;

                string enumScreenSize = string.Format("res{0}x{1}", LogicPOSAppContext.ScreenSize.Width, LogicPOSAppContext.ScreenSize.Height);

                try
                {
                    ScreenSize screenSizeEnum = (ScreenSize)Enum.Parse(typeof(ScreenSize), enumScreenSize, true);

                    if (appThemeCalcDynamicSize)
                    {
                        switch (screenSizeEnum)
                        {
                            case ScreenSize.res800x600:
                                sizePosBaseButtonSize = new Size(100, 75);
                                sizePosToolbarButtonSize = new Size(54, 38);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(22, 22);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 8";
                                fontTicketListColumn = "8";
                                fontDocumentsSizeDefault = "6";
                                break;
                            case ScreenSize.res1024x600:
                            case ScreenSize.res1024x768:
                                sizePosBaseButtonSize = new Size(120, 90);
                                sizePosToolbarButtonSize = new Size(80, 60);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(34, 34);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 9";
                                fontTicketListColumn = "9";
                                fontDocumentsSizeDefault = "6";
                                break;
                            case ScreenSize.res1152x864:
                            case ScreenSize.res1280x720:
                            case ScreenSize.res1280x768:
                            case ScreenSize.res1280x800:
                            case ScreenSize.res1280x1024:
                            case ScreenSize.res1360x768:
                            case ScreenSize.res1366x768:
                            case ScreenSize.res1440x900:
                            case ScreenSize.res1536x864:
                            case ScreenSize.res1600x900:
                                fontTicketListColumnTitle = "Bold 10";
                                fontTicketListColumn = "10";
                                sizePosBaseButtonSize = new Size(140, 105);
                                sizePosToolbarButtonSize = new Size(100, 75);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(42, 42);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontDocumentsSizeDefault = "8";
                                break;
                            case ScreenSize.res1680x1050:
                            case ScreenSize.res1920x1080:
                            case ScreenSize.res1920x1200:
                            case ScreenSize.res2160x1440:
                            case ScreenSize.res2560x1080:
                            case ScreenSize.res2560x1440:
                            case ScreenSize.res3440x1440:
                            case ScreenSize.res3840x2160:
                                sizePosBaseButtonSize = new Size(160, 120);
                                sizePosToolbarButtonSize = new Size(120, 90);
                                sizePosTicketPadButtonSize = new Size(sizePosToolbarButtonSize.Width, sizePosToolbarButtonSize.Height);
                                sizePosTicketPadButtonDoubleWidth = new Size(sizePosToolbarButtonSize.Width * 2, sizePosToolbarButtonSize.Height);
                                sizePosToolbarButtonIconSize = new Size(50, 50);
                                sizePosTicketPadButtonIconSize = new Size(sizePosToolbarButtonIconSize.Width, sizePosToolbarButtonIconSize.Height);
                                fontTicketListColumnTitle = "Bold 10";
                                fontTicketListColumn = "10";
                                fontDocumentsSizeDefault = "10";
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
                                fontDocumentsSizeDefault = "6";
                                break;
                        }
                    }
                    else
                    {
                        // Use Defaults from Config
                        sizePosBaseButtonSize = AppSettings.Instance.sizePosBaseButton;
                        sizePosToolbarButtonSize = AppSettings.Instance.sizePosToolbarButton;
                        sizePosTicketPadButtonSize = AppSettings.Instance.sizePosTicketPadButton;
                        sizePosTicketPadButtonDoubleWidth = AppSettings.Instance.sizePosTicketPadButtonDoubleWidth;
                        sizePosToolbarButtonIconSize = AppSettings.Instance.sizePosToolbarButtonIcon;
                        sizePosTicketPadButtonIconSize = AppSettings.Instance.sizePosTicketPadButtonIcon;
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                    Utils.ShowMessageTouchUnsupportedResolutionDetectedAndExit(LoginWindow.Instance, LogicPOSAppContext.ScreenSize.Width, LogicPOSAppContext.ScreenSize.Height);
                }

                int posMainWindowComponentsMargin = AppSettings.Instance.intPosMainWindowComponentsMargin;
                int posMainWindowEventBoxPosTicketListColumnWidth = sizePosTicketPadButtonSize.Width * 4;
                int posMainWindowEventBoxStatusBar1And2Height = AppSettings.Instance.intPosMainWindowEventBoxStatusBar1And2Height;
                // Remove Margin, Column Price, Qnt, Total
                int posMainWindowEventBoxPosTicketListColumnsDesignationWidth = posMainWindowEventBoxPosTicketListColumnWidth - 10 - 65 - 55 - 75;

                //IN009257 Redimensionar botões para a resolução 1024 x 768.
                sizePosBaseButtonSizeDefault = sizePosBaseButtonSize;
                sizePosToolbarButtonSizeDefault = new Size((int)(sizePosToolbarButtonSize.Width / 1.4), (int)(sizePosToolbarButtonSize.Height / 1.4)); 
                sizePosTicketPadButtonSizeDefault = sizePosTicketPadButtonSize;
                sizePosTicketPadButtonDoubleWidthDefault = sizePosTicketPadButtonDoubleWidth;
                sizePosToolbarButtonIconSizeDefault = new Size((int)(sizePosToolbarButtonIconSize.Width / 1.7), (int)(sizePosToolbarButtonIconSize.Height / 1.7));
                sizePosTicketPadButtonIconSizeDefault = sizePosTicketPadButtonIconSize;
                //IN009257 END

                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadColumns", intEventBoxPosTicketPadColumns);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadRows", intEventBoxPosTicketPadRows);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonSize", sizePosTicketPadButtonSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonDoubleWidthSize", sizePosTicketPadButtonDoubleWidth);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowToolbarButtonSize", sizePosToolbarButtonSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowComponentsMargin", posMainWindowComponentsMargin);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnWidth", posMainWindowEventBoxPosTicketListColumnWidth);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxStatusBar1And2Height", posMainWindowEventBoxStatusBar1And2Height);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventboxToolbarIconSize", sizePosToolbarButtonIconSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketPadButtonsIconSize", sizePosTicketPadButtonIconSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnsDesignationWidth", posMainWindowEventBoxPosTicketListColumnsDesignationWidth);

                // Calculate TablePads TableConfig and Button Sizes, from starter sizePosBaseButton

                //Override this from Config
                int usefullAreaForMainTablePadsWidth = LogicPOSAppContext.ScreenSize.Width - posMainWindowComponentsMargin - (posMainWindowEventBoxPosTicketListColumnWidth + posMainWindowComponentsMargin * 2);
                int usefullAreaForMainTablePadsHeight = LogicPOSAppContext.ScreenSize.Height - (posMainWindowEventBoxStatusBar1And2Height * 2) - posMainWindowComponentsMargin - posMainWindowEventBoxStatusBar1And2Height - (sizePosToolbarButtonSize.Height + posMainWindowComponentsMargin * 2);
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

                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowBaseButtonSize", sizePosBaseButtonSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyTableConfig", tableConfigTablePadFamily);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyTableConfig", tableConfigTablePadSubFamily);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleTableConfig", tableConfigTablePadArticle);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowButtonFavoritesPosition", positionButtonFavorites);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyPosition", positionTablePadFamily);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyPosition", positionTablePadSubFamily);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticlePosition", positionTablePadArticle);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonPrevPosition", tablePadFamilyButtonPrevPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonNextPosition", tablePadFamilyButtonNextPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonPrevPosition", tablePadSubFamilyButtonPrevPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonNextPosition", tablePadSubFamilyButtonNextPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonPrevPosition", tablePadArticleButtonPrevPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonNextPosition", tablePadArticleButtonNextPosition);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowSmallButtonScrollerSize", sizePosSmallButtonScrollerSize);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnTitleFont", fontTicketListColumnTitle);
                LogicPOSAppContext.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnFont", fontTicketListColumn);

                // Test Expressions Here
                //string hardCodeExpression = "Size((tableConfigTablePadArticle.Columns - 2) * posMainWindowBaseButtonSize.Width, posMainWindowEventBoxStatusBar1And2Height)";
                //string hardCodeResult = GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString();
                //log.Debug(string.Format("result: [{0}]", GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString()));
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
        }
    }
}
