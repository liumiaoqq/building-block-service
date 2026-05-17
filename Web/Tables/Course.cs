namespace Web.Tables
{
    [Description("课程")]
    [YoungTable("Course")]
    public class Course : CreationAuditedAggregateRoot
    {
        [Description("标题")]
        public string Title { get; set; }
        [Description("封面")]

        public string Cover { get; set; }
        [Description("Bilibili链接")]

        public string BILIBILILinks { get; set; }
        [Description("排序")]
        public int Sort { get; set; }

        [Description("课程类型")]
        public Guid? CourseTypeId { get; set; }

        [Description("可得积分")]
        public int Points { get; set; }


        [Description("作者")]
        public string Author { get; set; }



    }
}
