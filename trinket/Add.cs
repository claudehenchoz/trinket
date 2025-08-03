using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ReverseMarkdown;

namespace trinket
{
    public partial class Add : Form
    {
        public Add()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close();
                return true;
            }
            
            if (keyData == (Keys.Control | Keys.Enter))
            {
                SaveAndClose();
                return true;
            }
            
            if (keyData == (Keys.Control | Keys.H))
            {
                InsertClipboardAsBlockquote();
                return true;
            }
            
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            SaveAndClose();
        }
        
        private void SaveAndClose()
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string trinketFolder = Path.Combine(documentsPath, "Trinket");
            
            // Create the Trinket folder if it doesn't exist
            if (!Directory.Exists(trinketFolder))
            {
                Directory.CreateDirectory(trinketFolder);
            }
            
            string filePath = Path.Combine(trinketFolder, Guid.NewGuid().ToString() + ".txt");
            StreamWriter streamWriter = new StreamWriter(filePath);
            streamWriter.Write(textBox1.Text.Trim());
            streamWriter.Close();
            this.Close();
        }

        private void Add_Shown(object sender, EventArgs e)
        {
            this.Activate();
        }

        private void InsertClipboardAsBlockquote()
        {
            try
            {
                IDataObject clipboardData = Clipboard.GetDataObject();
                if (clipboardData == null) return;

                string blockquoteText = "";
                string sourceUrl = "";

                // Check if clipboard contains HTML format
                if (clipboardData.GetDataPresent("HTML Format"))
                {
                    string htmlData = (string)clipboardData.GetData("HTML Format");
                    string htmlFragment = ExtractHtmlFragment(htmlData);
                    sourceUrl = ExtractSourceUrl(htmlData);

                    if (!string.IsNullOrEmpty(htmlFragment))
                    {
                        // Convert HTML to Markdown
                        var converter = new Converter();
                        string markdown = converter.Convert(htmlFragment);
                        blockquoteText = FormatAsBlockquote(markdown);
                    }
                }
                
                // If no HTML or HTML processing failed, try plain text
                if (string.IsNullOrEmpty(blockquoteText) && clipboardData.GetDataPresent(DataFormats.Text))
                {
                    string plainText = (string)clipboardData.GetData(DataFormats.Text);
                    if (!string.IsNullOrEmpty(plainText))
                    {
                        blockquoteText = FormatAsBlockquote(plainText);
                    }
                }

                // Insert the blockquote text into the textbox
                if (!string.IsNullOrEmpty(blockquoteText))
                {
                    int selectionStart = textBox1.SelectionStart;
                    string textToInsert = blockquoteText;
                    
                    // Add source URL if available
                    if (!string.IsNullOrEmpty(sourceUrl))
                    {
                        textToInsert += Environment.NewLine + Environment.NewLine + "[Source](" + sourceUrl + ")";
                    }
                    
                    textBox1.Text = textBox1.Text.Insert(selectionStart, textToInsert);
                    textBox1.SelectionStart = selectionStart + textToInsert.Length;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error processing clipboard: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private string ExtractHtmlFragment(string htmlData)
        {
            // HTML Format structure: StartHTML, EndHTML, StartFragment, EndFragment
            try
            {
                var lines = htmlData.Split('\n');
                int startFragment = -1, endFragment = -1;

                foreach (string line in lines)
                {
                    if (line.StartsWith("StartFragment:"))
                        int.TryParse(line.Substring(14), out startFragment);
                    else if (line.StartsWith("EndFragment:"))
                        int.TryParse(line.Substring(12), out endFragment);
                }

                if (startFragment >= 0 && endFragment > startFragment && endFragment <= htmlData.Length)
                {
                    return htmlData.Substring(startFragment, endFragment - startFragment);
                }
            }
            catch { }
            
            return "";
        }

        private string ExtractSourceUrl(string htmlData)
        {
            // Look for SourceURL in the HTML Format header
            try
            {
                var lines = htmlData.Split('\n');
                foreach (string line in lines)
                {
                    if (line.StartsWith("SourceURL:"))
                        return line.Substring(10).Trim();
                }
            }
            catch { }
            
            return "";
        }

        private string FormatAsBlockquote(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            
            var lines = text.Split(new[] { '\r', '\n' }, StringSplitOptions.None);
            var blockquoteLines = new List<string>();
            
            foreach (string line in lines)
            {
                blockquoteLines.Add("> " + line);
            }
            
            return string.Join(Environment.NewLine, blockquoteLines);
        }

    }
}
