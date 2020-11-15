namespace zInvoiceTransformer
{
    public class TemplateListItem
    {
        public string Name;
        public string Id;
        public bool IsInUse;
        public override string ToString()
        {
            return Name;
        }
    }
}