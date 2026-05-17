namespace Web.Tables
{
    [Description("课程类型")]
    [YoungTable("CourseType")]
    public class CourseType : CreationAuditedAggregateRoot
    {

        public string Name { get; set; }


        public int Sort { get; set; }


    }
}
