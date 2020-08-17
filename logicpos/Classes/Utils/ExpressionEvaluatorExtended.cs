using Gtk;
using logicpos.App;
using logicpos.Classes.Enums.App;
using logicpos.Classes.Logic.Others;
using logicpos.resources.Resources.Localization;
using System;
using System.Drawing;

namespace logicpos
{
    public class ExpressionEvaluatorExtended
    {
		//IN009257 Redimensionar botões para a resolução 1024 x 768
        public static Size sizePosBaseButtonSizeDefault;
        public static Size sizePosToolbarButtonSizeDefault;
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
                object widthResult = GlobalApp.ExpressionEvaluator.Evaluate(widthExpression);
                object heightResult = GlobalApp.ExpressionEvaluator.Evaluate(heightExpression);

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
                Size startupWindowObjectsTablePadUserMarginLeftTopSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsTablePadUserMarginLeftTop"]);//new Size(120, 120);// Add to Config
                Size startupWindowObjectsTablePadUserButtonSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsTablePadUserButton"]);//new Size(120, 102);// Add to Config
                Size startupWindowObjectsTablePadUserTablePadUserButtonPrevSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsTablePadUserTablePadUserButtonPrev"]);//new Size(120, 60);// Add to Config

                int startupWindowObjectsTablePadUserTableConfigRows = Convert.ToInt16(
                    (
                        GlobalApp.ScreenSize.Height
                        // Margin Top/Bottom
                        - (startupWindowObjectsTablePadUserMarginLeftTopSize.Height * 2)
                        - (startupWindowObjectsTablePadUserTablePadUserButtonPrevSize.Height * 2)
                        )
                        / startupWindowObjectsTablePadUserButtonSize.Height
                    );

                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserMarginLeftTopSize", startupWindowObjectsTablePadUserMarginLeftTopSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserButtonSize", startupWindowObjectsTablePadUserButtonSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTablePadUserButtonPrevSize", startupWindowObjectsTablePadUserTablePadUserButtonPrevSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsTablePadUserTableConfigRows", startupWindowObjectsTablePadUserTableConfigRows);

                // startupWindow.Objects.NumberPadPin
                int startupWindowObjectsNumberPadPinRight = Convert.ToInt16(GlobalFramework.Settings["intStartupWindowObjectsNumberPadPinRight"]);//120;// Add to Config
                Size startupWindowObjectsNumberPadPinSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsNumberPadPin"]);//new Size(315, 434);// Add to Config
                Size startupWindowObjectsNumberPadPinButtonSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsNumberPadPinButton"]);//new Size(99, 59);// Add to Config

                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinRight", startupWindowObjectsNumberPadPinRight);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinSize", startupWindowObjectsNumberPadPinSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsNumberPadPinButtonSize", startupWindowObjectsNumberPadPinButtonSize);

                // startupWindow.Objects.LabelVersion
                Size startupWindowObjectsLabelVersionSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsLabelVersion"]);//new Size(307, 50);// Add to Config
                Size startupWindowObjectsLabelVersionSizeMarginRightBottomSize = Utils.StringToSize(GlobalFramework.Settings["sizeStartupWindowObjectsLabelVersionSizeMarginRightBottom"]);//new Size(124, 128);// Add to Config

                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSize", startupWindowObjectsLabelVersionSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("startupWindowObjectsLabelVersionSizeMarginRightBottomSize", startupWindowObjectsLabelVersionSizeMarginRightBottomSize);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }

            // Test Expressions Here
            //string hardCodeExpression = "";
            //string hardCodeResult = GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString();
            //log.Debug(string.Format("result: [{0}]", GlobalApp.ExpressionEvaluator.Evaluate(hardCodeExpression).ToString()));
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

                if (SettingsApp.IsDefaultTheme)
                {
                    //intEventBoxPosTicketPadColumns = 4;
                    intEventBoxPosTicketPadRows = 5;
                }

                // Detect Dynamic Size
                bool appThemeCalcDynamicSize = Convert.ToBoolean(GlobalFramework.Settings["appThemeCalcDynamicSize"]);
                Size sizePosBaseButtonSize = new Size(0, 0);
                Size sizePosToolbarButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonSize = new Size(0, 0);
                Size sizePosTicketPadButtonDoubleWidth = new Size(0, 0);
                Size sizePosToolbarButtonIconSize = new Size(0, 0);
                Size sizePosTicketPadButtonIconSize = new Size(0, 0);
                string sizePosBaseButtonString = GlobalFramework.Settings["sizePosBaseButton"].Replace(" ", string.Empty);
                string fontTicketListColumnTitle = string.Empty;
                string fontTicketListColumn = string.Empty;

                string enumScreenSize = string.Format("res{0}x{1}", GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);

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
                        sizePosBaseButtonSize = Utils.StringToSize(GlobalFramework.Settings["sizePosBaseButton"]);
                        sizePosToolbarButtonSize = Utils.StringToSize(GlobalFramework.Settings["sizePosToolbarButton"]);
                        sizePosTicketPadButtonSize = Utils.StringToSize(GlobalFramework.Settings["sizePosTicketPadButton"]);
                        sizePosTicketPadButtonDoubleWidth = Utils.StringToSize(GlobalFramework.Settings["sizePosTicketPadButtonDoubleWidth"]);
                        sizePosToolbarButtonIconSize = Utils.StringToSize(GlobalFramework.Settings["sizePosToolbarButtonIcon"]);
                        sizePosTicketPadButtonIconSize = Utils.StringToSize(GlobalFramework.Settings["sizePosTicketPadButtonIcon"]);
                    }
                }
                catch (Exception ex)
                {
                    log.Error(ex.Message, ex);
                    Utils.ShowMessageTouchUnsupportedResolutionDetectedAndExit(GlobalApp.WindowStartup, GlobalApp.ScreenSize.Width, GlobalApp.ScreenSize.Height);
                }

                int posMainWindowComponentsMargin = Convert.ToInt16(GlobalFramework.Settings["intPosMainWindowComponentsMargin"]);
                int posMainWindowEventBoxPosTicketListColumnWidth = sizePosTicketPadButtonSize.Width * 4;
                int posMainWindowEventBoxStatusBar1And2Height = Convert.ToInt16(GlobalFramework.Settings["intPosMainWindowEventBoxStatusBar1And2Height"]);
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

                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadColumns", intEventBoxPosTicketPadColumns);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadRows", intEventBoxPosTicketPadRows);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonSize", sizePosTicketPadButtonSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTicketPadButtonDoubleWidthSize", sizePosTicketPadButtonDoubleWidth);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowToolbarButtonSize", sizePosToolbarButtonSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowComponentsMargin", posMainWindowComponentsMargin);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnWidth", posMainWindowEventBoxPosTicketListColumnWidth);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxStatusBar1And2Height", posMainWindowEventBoxStatusBar1And2Height);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventboxToolbarIconSize", sizePosToolbarButtonIconSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketPadButtonsIconSize", sizePosTicketPadButtonIconSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListColumnsDesignationWidth", posMainWindowEventBoxPosTicketListColumnsDesignationWidth);

                // Calculate TablePads TableConfig and Button Sizes, from starter sizePosBaseButton

                //Override this from Config
                int usefullAreaForMainTablePadsWidth = GlobalApp.ScreenSize.Width - posMainWindowComponentsMargin - (posMainWindowEventBoxPosTicketListColumnWidth + posMainWindowComponentsMargin * 2);
                int usefullAreaForMainTablePadsHeight = GlobalApp.ScreenSize.Height - (posMainWindowEventBoxStatusBar1And2Height * 2) - posMainWindowComponentsMargin - posMainWindowEventBoxStatusBar1And2Height - (sizePosToolbarButtonSize.Height + posMainWindowComponentsMargin * 2);
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
                Position positionButtonFavorites = new Position(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + posMainWindowComponentsMargin);
                Position positionTablePadFamily = new Position(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + sizePosBaseButtonSize.Height + posMainWindowComponentsMargin);
                Position positionTablePadSubFamily = new Position(posMainWindowComponentsMargin + sizePosBaseButtonSize.Width, (posMainWindowEventBoxStatusBar1And2Height * 2) + posMainWindowComponentsMargin);
                Position positionTablePadArticle = new Position(posMainWindowComponentsMargin + sizePosBaseButtonSize.Width, (posMainWindowEventBoxStatusBar1And2Height * 2) + sizePosBaseButtonSize.Height + posMainWindowComponentsMargin);
                // ButtonNext/Prev
                Position tablePadFamilyButtonPrevPosition = new Position(posMainWindowComponentsMargin, posMainWindowEventBoxStatusBar1And2Height + posMainWindowComponentsMargin);
                Position tablePadFamilyButtonNextPosition = new Position(posMainWindowComponentsMargin, (posMainWindowEventBoxStatusBar1And2Height * 2) + (posMainWindowComponentsMargin / 2) + (sizePosBaseButtonSize.Height * guessedTablePadRows) + (posMainWindowComponentsMargin / 2));
                Position tablePadSubFamilyButtonPrevPosition = new Position(posMainWindowComponentsMargin + (sizePosBaseButtonSize.Width * (guessedTablePadColumns - 1)), tablePadFamilyButtonPrevPosition.Y);
                Position tablePadSubFamilyButtonNextPosition = new Position(posMainWindowComponentsMargin + (sizePosBaseButtonSize.Width * (guessedTablePadColumns - 1)) + (sizePosBaseButtonSize.Width / 2), tablePadFamilyButtonPrevPosition.Y);
                Position tablePadArticleButtonPrevPosition = new Position(tablePadSubFamilyButtonPrevPosition.X, tablePadFamilyButtonNextPosition.Y);
                Position tablePadArticleButtonNextPosition = new Position(tablePadSubFamilyButtonNextPosition.X, tablePadFamilyButtonNextPosition.Y);
                Size sizePosSmallButtonScrollerSize = new Size(Convert.ToInt16(sizePosBaseButtonSize.Width / 2), posMainWindowEventBoxStatusBar1And2Height);

                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowBaseButtonSize", sizePosBaseButtonSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyTableConfig", tableConfigTablePadFamily);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyTableConfig", tableConfigTablePadSubFamily);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleTableConfig", tableConfigTablePadArticle);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowButtonFavoritesPosition", positionButtonFavorites);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyPosition", positionTablePadFamily);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyPosition", positionTablePadSubFamily);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticlePosition", positionTablePadArticle);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonPrevPosition", tablePadFamilyButtonPrevPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadFamilyButtonNextPosition", tablePadFamilyButtonNextPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonPrevPosition", tablePadSubFamilyButtonPrevPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadSubFamilyButtonNextPosition", tablePadSubFamilyButtonNextPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonPrevPosition", tablePadArticleButtonPrevPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowTablePadArticleButtonNextPosition", tablePadArticleButtonNextPosition);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowSmallButtonScrollerSize", sizePosSmallButtonScrollerSize);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnTitleFont", fontTicketListColumnTitle);
                GlobalApp.ExpressionEvaluator.Variables.Add("posMainWindowEventBoxPosTicketListTicketListColumnFont", fontTicketListColumn);

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
