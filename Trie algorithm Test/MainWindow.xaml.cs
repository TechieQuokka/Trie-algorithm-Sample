using System.Linq;

namespace Trie_algorithm_Test
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : System.Windows.Window
    {
        private Collections.ITrie<char> trie = new Collections.Trie<char>();
        public MainWindow()
        {
            this.InitializeComponent();
            this.textBox.Focus();
            return;
        }

        private void AddText (System.Windows.Controls.RichTextBox textBox, string text)
        {
            var paragraph = new System.Windows.Documents.Paragraph();
            paragraph.Margin = new System.Windows.Thickness(0);
            paragraph.Inlines.Add(text);
            textBox.Document.Blocks.Add(paragraph);
        }

        private System.Collections.Generic.IList<(string destination, uint count)> CollectString (System.Collections.Generic.IEnumerable<(System.Collections.Generic.IEnumerable<char> destination, uint count)> data)
        {
            if (data is null) throw new System.ArgumentNullException(nameof(data));

            var buffer = new System.Collections.Generic.List<(string destination, uint count)>();
            foreach (var element in data)
            {
                buffer.Add((string.Join("", element.destination), element.count));
            }
            buffer.Sort((first, second) =>
            {
                int result = second.count.CompareTo(first.count);
                if (result != 0) return result;
                else return first.destination.CompareTo(second.destination);
            });
            return buffer;
        }

        private void SubmitButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = this.textBox.Text;
            if (text is "") return;

            this.trie.Insert(text);
            this.textBox.Text = "";

            var tree = this.trie as Collections.Trie<char> ?? throw new System.ArgumentNullException(nameof(this.trie), "The trie is null.");
            var texts = tree.FindAll();

            this.data_RichTextBox.Document.Blocks.Clear();
            this.result_richTextBox.Document.Blocks.Clear();
            foreach (var element in texts)
            {
                this.AddText(this.data_RichTextBox, string.Join("", element.destination) + " : " + element.count.ToString());
            }
            return;
        }

        private void textBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            var textBox = sender as System.Windows.Controls.TextBox ?? throw new System.ArgumentNullException(nameof(sender), "The sender is null");
            if (textBox.Text.Length < 1)
            {
                this.result_richTextBox.Document.Blocks.Clear();
                return;
            }

            this.result_richTextBox.Document.Blocks.Clear();
            var result = this.CollectString ((this.trie as Collections.Trie<char>)?.Find(textBox.Text));
            foreach (var text in result)
            {
                this.AddText(this.result_richTextBox, string.Join("", text.destination));
            }
            return;
        }

        private void textBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.F3)
            {
                this.Remove_Button_Click(this.Remove_Button, new System.Windows.RoutedEventArgs());
                return;
            }
            else if (e.Key != System.Windows.Input.Key.Enter) return;

            this.result_richTextBox.Document.Blocks.Clear();
            this.SubmitButton_Click(this.Submit_Button, new System.Windows.RoutedEventArgs());
            return;
        }

        private void Remove_Button_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            var text = this.textBox.Text;
            if (text.Length < 1) return;

            bool? result = (this.trie as Collections.Trie<char>)?.DecreaseFlagCount(textBox.Text);
            if (result is false) System.Windows.MessageBox.Show("Target deletion failed.. :(", "Warning!", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            else
            {
                var tree = this.trie as Collections.Trie<char> ?? throw new System.ArgumentNullException(nameof(this.trie), "The trie is null.");
                var texts = tree.FindAll();

                this.textBox.Text = string.Empty;
                this.data_RichTextBox.Document.Blocks.Clear();
                this.result_richTextBox.Document.Blocks.Clear();
                foreach (var element in texts)
                {
                    this.AddText(this.data_RichTextBox, string.Join("", element.destination) + " : " + element.count.ToString());
                }
            }
            return;
        }

        private void Window_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key is System.Windows.Input.Key.F4)
            {
                (this.trie as Collections.Trie<char>).Clear();
                this.textBox.Text = string.Empty;
                this.data_RichTextBox.Document.Blocks.Clear();
                this.result_richTextBox.Document.Blocks.Clear();
            }
            return;
        }
    }
}
