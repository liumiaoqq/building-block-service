

namespace Web.HttpClient
{
    public enum DeepSeekSystemPrompt
    {
        [Description("表结构长度调整")]
        TableStructureLengthAdjustment = 0,
        [Description("表结构名称驼峰命名")]
        TableStructureNameCamelCase = 1,
        [Description("表结构关系")]
        TableStructureRelationShip = 2,

        [Description("枚举解析")]
        EnumParse = 3,

        [Description("表列排序")]
        TableColumnSort = 4,

        [Description("项目结构生成")]
        ProjectStructureGeneration = 5,

        [Description("PlantUML流程图生成")]
        PlantUMLGeneration = 6,

        [Description("代码模块分类")]
        CodeModuleClassification = 7,

        [Description("枚举绑定")]
        EnumBinding = 9,

        [Description("列类型绑定")]
        ColumnPropTypeBinding = 10,

        [Description("主显示列选择")]
        PrimaryDisplayColumn = 11,

    }
}
