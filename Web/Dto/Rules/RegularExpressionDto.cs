namespace Web.Dto.Rules
{
    public class RegularExpressionDto
    {

        public RegularExpressionMatchType RegularExpressionMatchType;
        public string MatchRegular { get; set; }

        public string Message { get; set; }

        public string RegularValue { get; set; }
    }
}
