using logicpos.App;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace logicpos.Classes.Gui.Gtk.Widgets
{
    //Non-UI - Parse XML into VirtualKeyboard

    public class VirtualKeyBoard
    {
        //Log4Net
        private log4net.ILog _log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        //Public Properties
        List<List<VirtualKey>> _internalKeyBoard = new List<List<VirtualKey>>();
        public List<List<VirtualKey>> KeyBoard
        {
            get { return (_internalKeyBoard); }
            set { _internalKeyBoard = value; }
        }

        //Constructor
        public VirtualKeyBoard(String pFile)
        {
            InitKeyboard(pFile);
        }

        //Always Allocate Position, and create a VirtualKey in Allocated Position, next point a reference to VirtualKey
        private void ValidateKeyBoard(int pRowIndex, int pColIndex)
        {
            //Row : <List<VirtualKey>
            while (_internalKeyBoard.Count <= pRowIndex)
            {
                _internalKeyBoard.Add(new List<VirtualKey>());
            }
            //Col : List<List<VirtualKey>>
            while (_internalKeyBoard[pRowIndex].Count <= pColIndex)
            {
                _internalKeyBoard[pRowIndex].Add(new VirtualKey());
            }
        }

        //Add VirtualKey to VirtualKeyboard
        private bool AddKey(XmlReader reader)
        {
            String currentType = "";
            int currentRowIndex = 0;
            int currentColIndex = 0;
            int currentLevel = 0;
            bool result = false;

            try
            {
                //Get Attributes from Node
                if (reader.MoveToAttribute("type")) currentType = reader.ReadContentAsString();
                if (reader.MoveToAttribute("row")) currentRowIndex = reader.ReadContentAsInt();
                if (reader.MoveToAttribute("col")) currentColIndex = reader.ReadContentAsInt();
                if (reader.MoveToAttribute("level")) currentLevel = reader.ReadContentAsInt();

                //Always Validate if position exists, if not Allocate space to it
                ValidateKeyBoard(currentRowIndex, currentColIndex);

                //Init tmpSelectedKey
                VirtualKey tmpSelectedKey = null;

                //Create reference to position in VirtualKeyboard 
                tmpSelectedKey = _internalKeyBoard[currentRowIndex][currentColIndex];

                //Init key position
                if (tmpSelectedKey.RowIndex == -1) tmpSelectedKey.RowIndex = currentRowIndex;
                if (tmpSelectedKey.ColIndex == -1) tmpSelectedKey.ColIndex = currentColIndex;

                //Get Attributes from node and Assign to Leve Properties
                VirtualKeyProperties tmpProperties = new VirtualKeyProperties();
                if (reader.MoveToAttribute("glyph")) tmpProperties.Glyph = reader.ReadContentAsString();
                if (reader.MoveToAttribute("ibmid")) tmpProperties.IbmId = reader.ReadContentAsString();
                if (reader.MoveToAttribute("deadkey")) tmpProperties.IsDeadKey = reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute("diacritical")) tmpProperties.Diacritical = reader.ReadContentAsString();
                if (reader.MoveToAttribute("notengraved")) tmpProperties.IsNotEngraved = reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute("charactername")) tmpProperties.CharacterName = reader.ReadContentAsString();
                if (reader.MoveToAttribute("unicodeid")) tmpProperties.UnicodeId = reader.ReadContentAsString();
                if (reader.MoveToAttribute("keywidth")) tmpProperties.KeyWidth = reader.ReadContentAsInt();
                if (reader.MoveToAttribute("numpad")) tmpProperties.IsNumPad = reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute("hidel2")) tmpProperties.HideL2 = reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute("bold")) tmpProperties.IsBold = reader.ReadContentAsBoolean();
                if (reader.MoveToAttribute("halign")) tmpProperties.HAlign = reader.ReadContentAsString();

                switch (currentLevel)
                {
                    case 0:
                        tmpSelectedKey.Type = currentType;
                        tmpSelectedKey.L1 = tmpProperties;
                        break;
                    case 1:
                        tmpSelectedKey.L2 = tmpProperties;
                        break;
                    case 2:
                        tmpSelectedKey.L3 = tmpProperties;
                        break;
                    default:
                        throw new Exception("Invalid key level");
                }
            }
            catch (Exception ex)
            {
                _log.Error(string.Format("AddKey(): {0}", ex.Message), ex);
            }

            return (result);
        }

        //Initialize VirtualKeyboard form XML File
        private void InitKeyboard(String pFile)
        {
            bool debug = false;

            //Init XmlReaderSettings
            XmlReaderSettings settings = new XmlReaderSettings(); settings.IgnoreWhitespace = true;
            settings.IgnoreWhitespace = true;
            settings.IgnoreComments = true;

            //Start Parse Keyboard XML
            using (XmlReader reader = XmlReader.Create(pFile, settings))
            {
                while (reader.Read())
                {
                    // Only detect start elements.
                    if (reader.IsStartElement())
                    {
                        // Get element name and switch on it.
                        switch (reader.Name)
                        {
                            case "keyboard":
                                break;
                            case "key":
                                this.AddKey(reader);
                                break;
                        }
                    }
                }
            }
            if (debug) ExportToCsv();
        }

        //Render a CSV from VirtualKeyboard, usefull to check Keyboard Imports
        private void ExportToCsv(String file = "virtualkeyboard.csv")
        {
            List<VirtualKey> tmpKeyboardRow;
            VirtualKey tmpKey;

            TextWriter textWriter = new StreamWriter(FrameworkUtils.OSSlash(GlobalFramework.Path["temp"] + file));
            textWriter.WriteLine("type\tglyph\trow\tcol\tlevel\tibmid\tdeadkey\tdiacritical\tnotengraved\tcharactername\tunicodeid\tkeywidtht\tnumpad\thidel2\tbold\thalign");

            for (int i = 0; i < _internalKeyBoard.Count; i++)
            {
                tmpKeyboardRow = _internalKeyBoard[i];

                for (int j = 0; j < tmpKeyboardRow.Count; j++)
                {
                    tmpKey = tmpKeyboardRow[j];
                    if (tmpKey.L1 != null) textWriter.WriteLine(GetVirtualKeyCSVProperties(tmpKey.Type, i, j, 0, tmpKey.L1));
                    if (tmpKey.L2 != null) textWriter.WriteLine(GetVirtualKeyCSVProperties(tmpKey.Type, i, j, 1, tmpKey.L2));
                    if (tmpKey.L3 != null) textWriter.WriteLine(GetVirtualKeyCSVProperties(tmpKey.Type, i, j, 2, tmpKey.L3));
                }
            }
            textWriter.Close();
        }

        //Render a CSV String with VirtualKey properties, used in ExportToCsv
        private String GetVirtualKeyCSVProperties(String pType, int pRow, int pCol, int pLevel, VirtualKeyProperties pKeyProperties)
        {
            String result = "";
            if (pKeyProperties != null)
            {
                result += pType + "\t";
                if (pKeyProperties.Glyph != string.Empty) { result += pKeyProperties.Glyph + "\t"; } else { result += "\t"; };
                result += pRow + "\t";
                result += pCol + "\t";
                result += pLevel + "\t";
                if (pKeyProperties.IbmId != string.Empty) { result += pKeyProperties.IbmId + "\t"; } else { result += "\t"; };
                if (pKeyProperties.IsDeadKey) { result += pKeyProperties.IsDeadKey.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.Diacritical != string.Empty) { result += pKeyProperties.Diacritical + "\t"; } else { result += "\t"; };
                if (pKeyProperties.IsNotEngraved) { result += pKeyProperties.IsNotEngraved.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.CharacterName != string.Empty) { result += pKeyProperties.CharacterName + "\t"; } else { result += "\t"; };
                if (pKeyProperties.UnicodeId != string.Empty) { result += pKeyProperties.UnicodeId + "\t"; } else { result += "\t"; };
                if (pKeyProperties.KeyWidth > 0) { result += pKeyProperties.KeyWidth.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.IsNumPad) { result += pKeyProperties.IsNumPad.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.HideL2) { result += pKeyProperties.HideL2.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.IsBold) { result += pKeyProperties.IsBold.ToString() + "\t"; } else { result += "\t"; };
                if (pKeyProperties.HAlign != string.Empty) { result += pKeyProperties.HAlign + "\t"; } else { result += "\t"; };
            };
            return result;
        }
    }
}
