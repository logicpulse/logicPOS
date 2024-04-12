using DevExpress.Xpo;
using Gtk;
using logicpos.financial;
using logicpos.App;
using logicpos.datalayer.DataLayer.Xpo;
using logicpos.Classes.Gui.Gtk.BackOffice;
using System;
using System.Collections.Generic;
using System.Drawing;
using logicpos.Classes.Gui.Gtk.Widgets;
using logicpos.Classes.Gui.Gtk.WidgetsXPO;
using logicpos.resources.Resources.Localization;
using logicpos.shared;
using logicpos.Classes.Enums.Dialogs;
using logicpos.Classes.Gui.Gtk.Pos.Dialogs;
using logicpos.financial.library.Classes.Stocks;
using logicpos.datalayer.Enums;
using logicpos.datalayer.DataLayer.Xpo.Articles;
using logicpos.financial.library.Classes.Reports;

namespace logicpos.Classes.Gui.Gtk.WidgetsGeneric
{
    //TODO: Change class GenericCRUDWidgetList : List<GenericCRUDWidget> to Dictionary Where the Key is the FieldName ex Designation
    //TODO: Or Create Function to Get Field from List like GetGenericCRUDWidget(string pFieldName) and Loop and Return it, Or null

    //T (XPGuidObject|DataRow)
    internal abstract class GenericCRUDWidgetList<T> : List<GenericCRUDWidget<T>>
    {
        //Log4Net
        protected log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Private Members
        protected Dictionary<T, bool> _modifiedDataSourceRowObjects;
        //Public Custom CRUD Events
        public event EventHandler BeforeUpdate;
        public event EventHandler AfterUpdate;
        public event EventHandler BeforeInsert;
        public event EventHandler AfterInsert;
        public event EventHandler BeforeValidate;
        public event EventHandler AfterValidate;

        //ParameterLess Constructor
        public GenericCRUDWidgetList() { }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Abstract/Virtual Methods to be Implemented by SubClass/Child Classes

        //Must Be Overridden
        public abstract bool Save();

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
		//Documentos - Acerto de arredondamentos [IN:019542]
        public bool Modified(object pSource, object pTarget, Type pType, string pFieldName = "")
        {
            bool result;

            if (pSource == null && pTarget == null)
            {
            }
            if (pSource != null)
            {
                if(pType == typeof(decimal) && pFieldName == "Price")
                {
                    result = !(Math.Round(Convert.ToDecimal(FrameworkUtils.StringToDecimal(Convert.ToString(pSource))),2).Equals(Math.Round(Convert.ToDecimal(FrameworkUtils.StringToDecimal(Convert.ToString(pTarget))),2)));
                }
                else
                {
                    result = !(pSource.Equals(pTarget));
                }                
            }
            else if (pTarget != null)
            {
                result = !(pTarget.Equals(pSource));
            }
            else
            {
                result = false;
            };
            //_logger.Debug(string.Format("Modified:[{0}] pSource:[{1}], pTarget:[{2}], pType:[{3}]", result, pSource, pTarget, pType));
            return result;
        }

        public int GetFieldIndex(string pFieldName)
        {
            int result = -1;
            int index = -1;

            foreach (GenericCRUDWidget<T> item in this)
            {
                index++;
                if (item.FieldName == pFieldName)
                {
                    result = index;
                }
            }

            return result;
        }

        //Used to get Widget from WidgetList
        public GenericCRUDWidget<T> GetFieldWidget(string pFieldName)
        {
            int index = GetFieldIndex(pFieldName);
            if (index > -1)
            {
                return this[GetFieldIndex(pFieldName)];
            }
            else {
                return null;
            }
        }

        /// <summary>
        /// Validate fields
        /// </summary>
        /// <returns></returns>
        public bool ValidateRecord()
        {
            bool result = true;
            String currentFieldLabel;
            String invalidFields = string.Empty;

            //Fire Event
            OnBeforeValidate();

            foreach (GenericCRUDWidget<T> item in this)
            {
                if (item.Widget.Sensitive == true)
                {
                    //Debug Helper
                    //if (debug && item.Widget.GetType() == typeof(XPOComboBox))
                    //if (debug && item.Widget.GetType() == typeof(XPOEntryBoxSelectRecord<fin_article,TreeViewArticle>))
                    //{
                    //    _logger.Debug(string.Format("item.FieldName:[{0}], item.Widget.GetType():[{1}], item.FieldType:[{2}], item.FieldProperty:[{3}], item.Required:[{4}], item.ValidationRule:[{5}], item.Validated: [{6}]", item.FieldName, item.Widget.GetType(), item.FieldType, item.FieldProperty, item.Required, item.ValidationRule, item.Validated));
                    //}

                    //Widgets Validation With RegEX Rule
                    if (item.ValidationRule != string.Empty)
                    {
                        if (!item.Validated)
                        {
                            result = false;
                            currentFieldLabel = (item.Label != null) ? item.Label.Text : string.Format("[{0}]", item.FieldName);
                            //_logger.Debug(string.Format("ValidateRecord(): Validation Error in item.Field:[{0}] currentFieldLabel:[{1}]", item.FieldName, currentFieldLabel));
                            invalidFields += string.Format("{0}{1}{2}", Environment.NewLine, "* ", currentFieldLabel);
                        }
                    }
                    //Widgets Validation Without RegEX Rule
                    else
                    {
                        if (item.Required)
                        {
                            switch (item.Widget.GetType().Name)
                            {
                                case "FileChooserButton":
                                    if (((FileChooserButton)item.Widget).Filename == null)
                                    {
                                        result = false;
                                        invalidFields += string.Format("{0}{1}{2}", Environment.NewLine, "* ", item.FieldName);
                                    }
                                    break;
                            }
                        }
                    }
                }
            }

            //Fire Event
            OnAfterValidate();

            if (!result)
            {
                ResponseType response = logicpos.Utils.ShowMessageTouch(GlobalApp.WindowBackOffice, DialogFlags.DestroyWithParent | DialogFlags.Modal, new Size(500, 500), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "window_title_dialog_validation_error"), string.Format(resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "dialog_message_field_validation_error"), invalidFields));
            };

            return result;
        }

        /// <summary>
        /// Update XPOObject database Record, Works with Insert and Update, using DialogMode Enum
        /// </summary>
        /// <param name="pDialogMode"></param>
        /// <returns></returns>
        public bool UpdateRecord(DialogMode pDialogMode)
        {
            if (pDialogMode == DialogMode.Insert)
            {
                //Fire Event
                OnBeforeInsert();
            }
            else if (pDialogMode == DialogMode.Update)
            {
                //Fire Event
                OnBeforeUpdate();
            };
            bool modified = false;

            bool result;
            try
            {
                //Initalize Modified Dictionary
                _modifiedDataSourceRowObjects = new Dictionary<T, bool>();

                foreach (GenericCRUDWidget<T> item in this)
                {
                    if (item.Widget.Sensitive == true)
                    {
                        //Entry
                        if (item.Widget.GetType() == typeof(Entry))
                        {
                            //If Type Decimal and User uses "," replace it by "."
                            if (item.FieldType == typeof(decimal))
                            {
                                (item.Widget as Entry).Text = (item.Widget as Entry).Text.Replace(',', '.');
                            }

                            modified = Modified(item.GetMemberValue(), (item.Widget as Entry).Text, item.FieldType, item.FieldName);
                            if (modified)
                            {
                                if (!String.IsNullOrEmpty((item.Widget as Entry).Text))
                                {
                                    item.SetMemberValue(Convert.ChangeType((item.Widget as Entry).Text, item.FieldType, GlobalFramework.CurrentCultureNumberFormat));
                                }
                                else
                                {
                                    item.SetMemberValue(null);
                                };
                                //With Reflection Property
                                //item.Property.SetValue(item.Source, Convert.ChangeType((item.Widget as Entry).Text, item.FieldType, GlobalFramework.CurrentCultureNumberFormat));
                            };
                        }

                        //TextView
                        else if (item.Widget.GetType() == typeof(TextView))
                        {
                            modified = Modified(item.GetMemberValue(), (item.Widget as TextView).Buffer.Text, item.FieldType);
                            if (modified)
                            {
                                if (!String.IsNullOrEmpty((item.Widget as TextView).Buffer.Text))
                                {
                                    item.SetMemberValue(Convert.ChangeType((item.Widget as TextView).Buffer.Text, item.FieldType));
                                }
                                else
                                {
                                    item.SetMemberValue(null);
                                };
                            };
                        }

                        //EntryMultiline
                        else if (item.Widget.GetType() == typeof(EntryMultiline))
                        {
                            modified = Modified(item.GetMemberValue(), (item.Widget as EntryMultiline).Value.Text, item.FieldType);
                            if (modified)
                            {
                                if (!String.IsNullOrEmpty((item.Widget as EntryMultiline).Value.Text))
                                {
                                    item.SetMemberValue(Convert.ChangeType((item.Widget as EntryMultiline).Value.Text, item.FieldType));
                                }
                                else
                                {
                                    item.SetMemberValue(null);
                                };
                            };
                        }

                        //CheckButton
                        else if (item.Widget.GetType() == typeof(CheckButton))
                        {
                            modified = Modified(item.GetMemberValue(), (item.Widget as CheckButton).Active, item.FieldType);
                            if (modified)
                            {
                                item.SetMemberValue(Convert.ChangeType((item.Widget as CheckButton).Active, item.FieldType));
                            }
                        }

                        //XPOComboBox
                        else if (item.Widget.GetType() == typeof(XPOComboBox))
                        {
                            modified = Modified(item.GetMemberValue(), (item.Widget as XPOComboBox).Value, item.FieldType);
                            if (modified)
                            {
                                item.SetMemberValue(Convert.ChangeType((item.Widget as XPOComboBox).Value, item.FieldType));
                            }
                            //_logger.Debug(string.Format("UpdateRecord(): [{0}] [{1}]==[{2}] [{3}]", item.Field, item.Source.GetMemberValue(item.Field), (item.Widget as XPOComboBox).Value, item.FieldType));
                        }

                        //FileChooserButton
                        else if (item.Widget.GetType() == typeof(FileChooserButton))
                        {
                            String relativeFilename = (String)Convert.ChangeType((item.Widget as FileChooserButton).Filename, item.FieldType);
                            /* ERR201810#15 - Database backup issues */
                            //if (relativeFilename != null) relativeFilename = FrameworkUtils.RelativePath(relativeFilename);
                            modified = Modified(item.GetMemberValue(), relativeFilename, item.FieldType);
                            if (modified)
                            {
                                item.SetMemberValue(relativeFilename);
                            }
                        }

                        //EntryBoxValidation
                        else if (item.Widget.GetType() == typeof(EntryBoxValidation))
                        {
                            modified = Modified(item.GetMemberValue(), (item.Widget as EntryBoxValidation).EntryValidation.Text, item.FieldType, item.FieldName);
                            if (modified)
                            {
                                if (!String.IsNullOrEmpty((item.Widget as EntryBoxValidation).EntryValidation.Text))
                                {
                                    //_logger.Debug(string.Format("Message1: [{0}/{1}/{2}/{3}]", item.FieldType, item.FieldName, (item.Widget as EntryBoxValidation).EntryValidation.Text, GlobalFramework.CurrentCultureNumberFormat));
                                    //Extra protection to convert string to Decimal, else may occur errors when work with en-US
                                    if (item.FieldType == typeof(decimal))
                                    {
                                        item.SetMemberValue(FrameworkUtils.StringToDecimal((item.Widget as EntryBoxValidation).EntryValidation.Text));
                                    }
                                    else
                                    {
                                        item.SetMemberValue(Convert.ChangeType((item.Widget as EntryBoxValidation).EntryValidation.Text, item.FieldType));
                                    }
                                }
                                else
                                {
                                    item.SetMemberValue(null);
                                };
                            };
                        }

                        //XPOEntryBoxSelectRecord
                        else if (item.Widget.GetType().IsGenericType && item.Widget.GetType().GetGenericTypeDefinition() == typeof(XPOEntryBoxSelectRecord<,>))
                        {
                            //Required to use Dynamic, we cant cast without knowing generic types <T,T>
                            dynamic dynamicWiget = item.Widget;

                            //Detect if Widget Value is a XPGuidObject SubClass, and Target is not a XPGuidObject
                            if (dynamicWiget.Value == null)
                            {
                                item.SetMemberValue(null);
                            }
                            else if (dynamicWiget.Value.GetType().BaseType == typeof(XPGuidObject) && item.FieldType == typeof(Guid))
                            {
                                modified = Modified(item.GetMemberValue(), dynamicWiget.Value.Oid, item.FieldType);
                                if (modified)
                                {
                                    item.SetMemberValue(dynamicWiget.Value.Oid);
                                }
                            }
                            else
                            {
                                modified = Modified(item.GetMemberValue(), dynamicWiget.Value, item.FieldType);
                                if (modified)
                                {
                                    item.SetMemberValue(dynamicWiget.Value);
                                }
                            }
                        }
                        //XPOEntryBoxSelectRecordValidation
                        else if (item.Widget.GetType().IsGenericType && item.Widget.GetType().GetGenericTypeDefinition() == typeof(XPOEntryBoxSelectRecordValidation<,>))
                        {
                            //Required to use Dynamic, we cant cast without knowing generic types <T,T>
                            dynamic dynamicWiget = item.Widget;

                            //Detect if Widget Value is a XPGuidObject SubClass, and Target is not a XPGuidObject
                            if (dynamicWiget.Value == null)
                            {
                                item.SetMemberValue(null);
                            }
                            else if (dynamicWiget.Value.GetType().BaseType == typeof(XPGuidObject) && item.FieldType == typeof(Guid))
                            {
                                modified = Modified(item.GetMemberValue(), dynamicWiget.Value.Oid, item.FieldType);
                                if (modified)
                                {
                                    item.SetMemberValue(dynamicWiget.Value.Oid);
                                }
                            }
                            else
                            {
                                modified = Modified(item.GetMemberValue(), dynamicWiget.Value, item.FieldType);
                                if (modified)
                                {
                                    item.SetMemberValue(dynamicWiget.Value);
                                }
                            }
                        };

                        //Force Modified if in in InsertMode
                        if (pDialogMode == DialogMode.Insert)
                        {
                            modified = true;
                        }

                        //Force Modified if Articles
                        //if(item.GetType() == typeof(fin_article))
                        //{
                        //    modified = true;
                        //}
                        //Check Modified
                        if (modified)
                        {
                            //_logger.Debug(string.Format("UpdateRecord(): Modified Object.Field: {0}{1}", item.Source.GetType().Name, item.Field));
                            try
                            {
                                //Add DataSourceRowObject to Modified Objects Stack
                                if (!_modifiedDataSourceRowObjects.ContainsKey(item.DataSourceRow)) _modifiedDataSourceRowObjects[item.DataSourceRow] = true;
                                modified = false;
                            }
                            catch (Exception ex)
                            {
                                _logger.Error(string.Format("UpdateRecord(): Error! Saving Field [{0}]/[{1}]: {0}", item.FieldName, item.DataSourceRow.ToString(), ex.Message), ex);
                                result = false;
                            };
                        };
                    };
                };

                if (_modifiedDataSourceRowObjects.Count > 0)
                {
                    //Call Child Implementation (XPO|DataTable)
                    result = Save();
                }
                else
                {
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                logicpos.Utils.ShowMessageTouch(GlobalApp.WindowStartup, DialogFlags.Modal, new Size(600, 350), MessageType.Error, ButtonsType.Ok, resources.CustomResources.GetCustomResources(GlobalFramework.Settings["customCultureResourceDefinition"], "global_error"), ex.Message);
                result = false;
            }

            if (result && pDialogMode == DialogMode.Insert)
            {
                //Fire Event
                OnAfterInsert();
            }
            else if (result && pDialogMode == DialogMode.Update)
            {
                //Fire Event
                OnAfterUpdate();
            };

            return result;
        }

        /// <summary>
        /// Handle Dialog OnResponse Event, to prevent Dialog Close  
        /// </summary>
        /// <param name="pDialog"></param>
        /// <param name="pDialogMode"></param>
        /// <param name="pResponse"></param>
        public void ProcessDialogResponse(Dialog pDialog, DialogMode pDialogMode, ResponseType pResponse)
        {
            if (pResponse == ResponseType.Ok)
            {
                if (pDialog.GetType() == typeof(DialogAddArticleStock) && GlobalFramework.LicenceModuleStocks)
                {
                    ProcessArticleStockParameter res = DialogAddArticleStock.GetProcessArticleStockParameter(pDialog as DialogAddArticleStock);
                    List<fin_articleserialnumber> barCodeLabelList = new List<fin_articleserialnumber>();

                    if (res != null)
                    {
                        if (res.ArticleCollection.Count > 0)
                        {
                            foreach (var item in res.ArticleCollection)
                            {
                                res.Quantity = item.Value.Item1;
                                
                                res.PurchasePrice = item.Value.Item3;
                                res.Article = item.Key;
                                res.WarehouseLocation = item.Value.Item4;
                                //If has serial Numbers
                                if (item.Value.Item2.Count > 0)
                                {
                                    foreach (var itemS in item.Value.Item2)
                                    {
                                        res.Quantity = 1;
                                        res.SerialNumber = itemS.Key;
                                        res.AssociatedArticles = itemS.Value;
                                        GlobalFramework.StockManagementModule.Add(ProcessArticleStockMode.In, res);
                                        string sql = string.Format("SELECT Oid FROM fin_articleserialnumber WHERE SerialNumber = '{0}';", res.SerialNumber.ToString());
                                        var serialNumberOid = GlobalFramework.SessionXpo.ExecuteScalar(sql);
                                        if (serialNumberOid != null)
                                        {
                                            fin_articleserialnumber serialNumber = (fin_articleserialnumber)GlobalFramework.SessionXpo.GetObjectByKey(typeof(fin_articleserialnumber), Guid.Parse(serialNumberOid.ToString()));
                                            barCodeLabelList.Add(serialNumber);
                                        }
                                    }
                                }
                                else {
                                    GlobalFramework.StockManagementModule.Add(ProcessArticleStockMode.In, res);
                                }
                                if (barCodeLabelList.Count > 0)
                                {
                                    CustomReport.ProcessReportBarcodeLabel(shared.Enums.CustomReportDisplayMode.Print, null, "", false, barCodeLabelList);
                                }
                            }
                            return;
                        }
                    }
                    else return;
                }
                //Validate Fields
                else if (!ValidateRecord())
                {
                    pDialog.Run();
                }
                else
                {
                    //Save Record
                    if (!UpdateRecord(pDialogMode))
                    {
                        pDialog.Run();
                    };
                    if (pDialog.GetType() == typeof(DialogArticleStock))
                    {
                        (pDialog as DialogArticleStock).TreeViewXPO_ArticleDetails.Refresh();
                        (pDialog as DialogArticleStock).TreeViewXPO_ArticleHistory.Refresh();
                        (pDialog as DialogArticleStock).TreeViewXPO_ArticleWarehouse.Refresh();
                        (pDialog as DialogArticleStock).TreeViewXPO_StockMov.Refresh();
                    }
                };
            };
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //Custom CRUD EventHandlers
        private void OnBeforeInsert()
        {
            BeforeInsert?.Invoke(this, EventArgs.Empty);
        }

        private void OnAfterInsert()
        {
            AfterInsert?.Invoke(this, EventArgs.Empty);
        }

        private void OnBeforeUpdate()
        {
            BeforeUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void OnAfterUpdate()
        {
            AfterUpdate?.Invoke(this, EventArgs.Empty);
        }

        private void OnBeforeValidate()
        {
            BeforeValidate?.Invoke(this, EventArgs.Empty);
        }

        private void OnAfterValidate()
        {
            AfterValidate?.Invoke(this, EventArgs.Empty);
        }
    }
}
