using System.ComponentModel;

namespace Web.Tables
{
    public enum RoleType
    {

        [Description("系统管理员")]
        系统管理员 = 0,
        [Description("普通会员")]
        普通会员 = 1,
        [Description("用户")]
        用户 = 2,
    }




    public enum PayWay
    {
        [Description("微信")]
        微信 = 0,

        [Description("支付宝")]
        支付宝 = 1,
        [Description("闲鱼")]
        闲鱼 = 2,
        [Description("淘宝")]
        淘宝 = 3,

    }

    public enum LanguageWay
    {
        [Description("Java")]
        Java = 0,
        [Description("Net")]
        Net = 1,
        [Description("NetCore")]
        NetCore = 2,
        [Description("ElementUI")]
        ElementUI = 3,
        [Description("LayUi")]
        LayUi = 4,
        [Description("UniApp")]
        UniApp = 5,
        [Description("Django")]
        Django = 6,
        [Description("SpringBoot")]
        SpringBoot = 7,
        [Description("Flask")]
        Flask = 8,

    }

    public enum ComponentType
    {
        [Description("框架组件")]
        框架组件 = 0,
        [Description("自定义组件")]
        自定义组件 = 1,
        [Description("自动化组件")]
        自动化组件 = 2,
    }



    public enum ColumnPropType
    {
        [Description("String")]
        字符串 = 0,
        [Description("Double")]
        双浮点型 = 1,
        [Description("Int")]
        整型 = 2,
        [Description("Decimal")]
        小数点 = 3,
        [Description("Bool")]
        布尔型 = 4,
        [Description("Enum")]
        枚举型 = 5,
        [Description("Date")]
        日期 = 6,
        [Description("DateTime")]
        时间 = 7,
        [Description("Text")]
        长文本 = 8,
        [Description("DateTimeRange")]
        起始截至时间 = 9,
        [Description("DateRange")]
        起始截至日期 = 10,

        [Description("MultiText")]
        多行文本 = 11,

        [Description("Image")]
        图片 = 12,

        [Description("Video")]
        视频 = 13,

        [Description("Audio")]
        音频 = 14,

        [Description("File")]
        文件 = 15,

    }



    public enum DataBaseType
    {
        [Description("SqlServer")]
        SqlServer = 0,
        [Description("MySql")]
        MySql = 1,
        [Description("SqLite")]
        SqLite = 2,

    }



    public enum TableNavigateType
    {
        [Description("一对一")]
        OneToOne = 0,
        // [Description("一对多")]
        // OneToMany = 1,
        //[Description("多对多")]
        //ManyToMany = 2,

    }

    public enum LookType
    {
        [Description("按计划")]
        Plan = 0,
        [Description("按模块")]
        Module = 1,

    }
    public enum SystemModuleType
    {

        [Description("功能")]
        FunctionalModule = 0,
        [Description("框架")]
        Frame = 1,
        [Description("模板")]
        Templete = 2,
        [Description("资源")]
        Source = 3,
    }
    public enum PlanType
    {

        [Description("系统")]
        System = 0,
        [Description("功能模块")]
        FunctionalModule = 1,
        [Description("模板")]
        Templete = 2,
        [Description("测试")]
        Test = 3,
        [Description("教学系统")]
        TeachingSystem = 4,

    }

    /// <summary>
    /// 导出规则匹配类型
    /// </summary>
    public enum ExportRuleMatchType
    {
        [Description("匹配属性名称")]
        匹配属性名称 = 1,
        [Description("匹配属性值")]
        匹配属性值 = 2,
        [Description("匹配属性类型")]
        匹配属性类型 = 3,

    }

    /// <summary>
    /// 导出规则匹配后处理方式
    /// </summary>
    public enum ExportRuleDispatchType
    {
        [Description("显示")]
        显示 = 1,
        [Description("隐藏")]
        隐藏 = 2,
        [Description("外键显示")]
        外键显示 = 3,

    }


    /// <summary>
    /// 编辑规则匹配类型
    /// </summary>
    public enum EditRuleMatchType
    {
        [Description("匹配属性名称")]
        匹配属性名称 = 1,
        [Description("匹配属性值")]
        匹配属性值 = 2,
        [Description("匹配属性类型")]
        匹配属性类型 = 3,
        [Description("匹配属性长度")]
        匹配属性长度 = 4,

    }



    /// <summary>
    /// 正则表达式要匹配的对象
    /// </summary>
    public enum RegularExpressionMatchType
    {
        [Description("匹配属性名称")]
        匹配属性名称 = 1,
        [Description("匹配属性值")]
        匹配属性值 = 2,
        [Description("匹配属性类型")]
        匹配属性类型 = 3,

    }

    /// <summary>
    /// 编辑规则匹配类型
    /// </summary>
    public enum EditFormType
    {
        [Description("输入框")]
        Input = 1,
        [Description("单选下拉框")]
        SigleSelect = 2,
        [Description("图片")]
        Image = 3,
        [Description("时间选择器")]
        DateTimePicker = 4,
        [Description("富文本")]
        RichText = 5,
        [Description("数据范围输入框")]
        QtyRange = 6,
        [Description("日期选择器")]
        DatePicker = 7,
        [Description("评分")]
        Rate = 8,
        [Description("文件上传")]
        FileUpload = 9,
        [Description("开关")]
        Switch = 10,
        [Description("颜色选择器")]
        ColorPicker = 11,
        [Description("滑块")]
        Slider = 12,
        [Description("日期范围输入框")]
        DateRange = 13,
        [Description("时间范围输入框")]
        DateTimeRange = 14,
        [Description("省市区输入框")]
        PCZInput = 15,
        [Description("音频上传")]
        MusicUpload = 16,
        [Description("视频上传")]
        VideoUpload = 17,
        [Description("经纬度选择器")]
        LatLngSelect = 18,
        [Description("长文本")]
        LongText = 19,
        [Description("多选下拉框")]
        MulitSelect = 20,
        [Description("复选框")]
        CheckBox = 21,
        [Description("单选框")]
        Radio = 22,
        [Description("隐藏")]
        Hidden = 23

    }


    public enum SelectLabelType
    {

        [Description("静态")]
        静态 = 1,
        [Description("动态")]
        动态 = 2,


    }


    /// <summary>
    /// 视图规则匹配类型
    /// </summary>
    public enum ViewRuleMatchType
    {
        [Description("匹配属性名称")]
        匹配属性名称 = 1,
        [Description("匹配属性值")]
        匹配属性值 = 2,
        [Description("匹配属性类型")]
        匹配属性类型 = 3,

    }
    /// <summary>
    /// 视图规则匹配类型
    /// </summary>
    public enum ViewColumnType
    {
        [Description("手机")]
        Phone = 1,
        [Description("多图片,切割")]
        SplitImages = 2,
        [Description("文件下载列表")]
        FILESLINK = 3,
        [Description("链接")]
        Link = 4,
        [Description("单图片")]
        Image = 5,
        [Description("时间")]
        DateTime = 6,
        [Description("音频列表")]
        Audio = 7,
        [Description("日期")]
        Date = 8,
        [Description("文本")]
        Text = 9,
        [Description("隐藏")]
        Hidden = 10,
        [Description("富文本")]
        RichText = 11,
        [Description("视频列表")]
        Video = 12,
        [Description("时间范围")]
        DateTimeRange = 13,
        [Description("日期范围")]
        DateRange = 14,
        [Description("判断")]
        JUDGMENTTAG = 15,
    }



    /// <summary>
    /// 搜索规则匹配类型
    /// </summary>
    public enum SearchRuleMatchType
    {
        [Description("匹配属性名称")]
        匹配属性名称 = 1,
        [Description("匹配属性值")]
        匹配属性值 = 2,
        [Description("匹配属性类型")]
        匹配属性类型 = 3,

    }
    /// <summary>
    /// 搜索规则匹配类型
    /// </summary>
    public enum SearchType
    {
        [Description("等于文本")]
        EqualString = 1,
        [Description("模糊文本")]
        LikeString = 2,
        [Description("单选下拉框")]
        SigleSelect = 3,
        [Description("多选下拉框")]
        MulitSelect = 4,
        [Description("单时间范围")]
        DateTimeRange = 5,
        [Description("单日期范围")]
        DateRange = 6,
        [Description("等于判断")]
        BoolSelect = 7,
        [Description("隐藏处理")]
        Hidden = 8,
        [Description("多时间范围")]
        MulitDateTimeRange = 9,
        [Description("多日期范围")]
        MulitDateRange = 10,

    }


    /// <summary>
    /// 用户选择模板
    /// </summary>
    public enum UserPlanTemplete
    {
        [Description("SpringBoot3+Vue2")]
        SpringBoot3Vue2 = 1,
        [Description("SpringBoot3+Vue3")]
        SpringBoot3Vue3 = 2,
    }



    /// <summary>
    /// 转换类型
    /// </summary>
    public enum SqlParseType
    {
        /// <summary>
        /// SQl转换成ER图
        /// </summary>
        [Description("SqlParseEr")]
        SqlParseEr = 0,
    }



    /// <summary>
    /// 项目案例类型
    /// </summary>
    public enum ProjectCaseType
    {
        [Description("基础脚手架")]
        基础脚手架 = 0,

        [Description("完整脚手架")]
        完整脚手架 = 1,

    }
    /// <summary>
    /// 项目亮点环境
    /// </summary>
    public enum ProjectHighlightEnvironment
    {
        [Description("通用")]
        通用 = 0,

        [Description("仅网页")]
        仅网页 = 1,
        [Description("仅小程序")]
        仅小程序 = 2,
    }




    /// <summary>
    /// 项目服务业务订单状态
    /// </summary>
    public enum ProjectServiceBusinessOrderStatus
    {
        [Description("关闭")]
        关闭 = 0,

        [Description("待支付")]
        待支付 = 1,
        [Description("待服务")]
        待服务 = 2,
        [Description("服务完成")]
        服务完成 = 3,

    }


    /// <summary>
    /// 积分记录类型
    /// </summary>
    public enum IntegralRecordType
    {
        [Description("每日签到")]
        每日签到 = 0,
        [Description("邀请注册")]
        邀请注册 = 1,
        [Description("视频打卡")]
        视频打卡 = 2,

    }

    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayType
    {
        [Description("积分")]
        积分 = 0,
        [Description("免费")]
        免费 = 1,
        [Description("微信")]
        微信 = 3,

    }
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum CourseAuditStatus
    {
        [Description("待提交")]
        待提交 = 0,
        [Description("审核中")]
        审核中 = 1,
        [Description("审核拒绝")]
        审核拒绝 = 2,
        [Description("审核通过")]
        审核通过 = 3,

        [Description("作废")]
        作废 = 4,
    }

    /// <summary>
    /// 画图类型
    /// </summary>
    public enum DrawingType
    {
        [Description("ER图")]
        ER图 = 0,
        [Description("流程图")]
        流程图 = 1,
        [Description("时序图")]
        时序图 = 2,
        [Description("用例图")]
        用例图 = 3,
        [Description("三线表")]
        三线表 = 4,
        [Description("项目结构图")]
        项目结构图 = 5,
    }
}
