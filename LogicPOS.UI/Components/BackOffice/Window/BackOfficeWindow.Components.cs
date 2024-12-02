using LogicPOS.Globalization;
using LogicPOS.UI.Buttons;

namespace LogicPOS.UI.Components.Windows
{
    public partial class BackOfficeWindow
    {
        public XAccordionParentButton BtnDocumentsSection = new XAccordionParentButton(LocalizedString.Instance["dialog_button_label_select_record_finance_documents"],
                                                                                      "Assets/Images/Icons/Accordion/pos_backoffice_documentos.png");

        public XAccordionParentButton BtnReportsSection = new XAccordionParentButton(LocalizedString.Instance["global_reports"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_relatorios.png");

        public XAccordionParentButton BtnArticlesSection = new XAccordionParentButton(LocalizedString.Instance["global_articles"],
                                                                                      "Assets/Images/Icons/Accordion/pos_backoffice_artigos.png");

        public XAccordionParentButton BtnFiscalSection = new XAccordionParentButton(LocalizedString.Instance["global_documents"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_informacao_fiscal.png");

        public XAccordionParentButton BtnCustomersSection = new XAccordionParentButton(LocalizedString.Instance["global_customers"],
                                                                                       "Assets/Images/Icons/Accordion/pos_backoffice_clientes.png");

        public XAccordionParentButton UsersSection = new XAccordionParentButton(LocalizedString.Instance["global_users"],
                                                                               "Assets/Images/Icons/Accordion/pos_backoffice_utilizadores.png");

        public XAccordionParentButton BtnDevicesSection = new XAccordionParentButton(LocalizedString.Instance["global_devices"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_impressoras.png");

        public XAccordionParentButton BtnOthersSection = new XAccordionParentButton(LocalizedString.Instance["global_other_tables"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_outras_tabelas.png");

        public XAccordionParentButton BtnConfigurationSection = new XAccordionParentButton(LocalizedString.Instance["global_configuration"],
                                                                                           "Assets/Images/Icons/Accordion/pos_backoffice_configuracao.png");

        public XAccordionParentButton BtnImportSection = new XAccordionParentButton(LocalizedString.Instance["global_import"],
                                                                                     "Assets/Images/Icons/Accordion/pos_backoffice_import.png");

        public XAccordionParentButton BtnExportSection = new XAccordionParentButton(LocalizedString.Instance["global_export"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_export.png");

        public XAccordionParentButton BtnSystemSection = new XAccordionParentButton(LocalizedString.Instance["global_system"],
                                                                                    "Assets/Images/Icons/Accordion/pos_backoffice_sistema.png");


        private void AddDocumentsSection()
        {
            PanelButtons.PackStart(BtnDocumentsSection.Button, false, false, 0);
        }

        private void AddReportsSection()
        {
            PanelButtons.PackStart(BtnReportsSection.Button, false, false, 0);
        }

        private void AddArticlesSection()
        {
            PanelButtons.PackStart(BtnArticlesSection.Button, false, false, 0);
        }

        private void AddFiscalSection()
        {
            PanelButtons.PackStart(BtnFiscalSection.Button, false, false, 0);
        }

        private void AddCustomersSection()
        {
            PanelButtons.PackStart(BtnCustomersSection.Button, false, false, 0);
        }

        private void AddUsersSection()
        {
            PanelButtons.PackStart(UsersSection.Button, false, false, 0);
        }

        private void AddDevicesSection()
        {
            PanelButtons.PackStart(BtnDevicesSection.Button, false, false, 0);
        }

        private void AddOthersSection()
        {
            PanelButtons.PackStart(BtnOthersSection.Button, false, false, 0);
        }

        private void AddConfigurationSection()
        {
            PanelButtons.PackStart(BtnConfigurationSection.Button, false, false, 0);
        }

        private void AddImportSection()
        {
            PanelButtons.PackStart(BtnImportSection.Button, false, false, 0);
        }

        private void AddExportSection()
        {
            PanelButtons.PackStart(BtnExportSection.Button, false, false, 0);
        }

        private void AddSystemSection()
        {
            PanelButtons.PackStart(BtnSystemSection.Button, false, false, 0);
        }

        private void AddSections()
        {
            AddDocumentsSection();
            AddReportsSection();
            AddArticlesSection();
            AddFiscalSection();
            AddCustomersSection();
            AddUsersSection();
            AddDevicesSection();
            AddOthersSection();
            AddConfigurationSection();
            AddImportSection();
            AddExportSection();
            AddSystemSection();
        }
    }
}
