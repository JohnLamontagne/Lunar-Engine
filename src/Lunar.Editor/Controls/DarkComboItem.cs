namespace Lunar.Editor.Controls
{
    public class DarkComboItem
    {
        public string Text { get; set; }

        public object Tag { get; set; }

        public DarkComboItem(string displayText)
        {
            this.Text = displayText;
        }

        public override string ToString()
        {
            return this.Text;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj) || obj.ToString() == this.ToString();
        }
    }
}