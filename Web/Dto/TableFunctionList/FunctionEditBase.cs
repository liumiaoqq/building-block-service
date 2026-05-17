namespace Web.Dto.TableFunctionList
{
    public   class FunctionEditBase
    {
        public EditFormType EditFormType { get;  set; }
        public string EditFormTypeFormat => EditFormType.ToDescription();

        public string EditFormTypeValue => EditFormType.ToString();

        public int SpanValue { get; set; }


        public bool IsRequired { get; set; }

        public bool EditDisabled { get; set; }

        public List<FunctionValidRule> ValidRules { get; set; }


        public bool IsClearable { get; set; }
    }
}
