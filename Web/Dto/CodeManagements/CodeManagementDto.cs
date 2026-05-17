using System;
using System.Collections.Generic;

namespace Web
{
    #region 代码管理相关DTO

    /// <summary>
    /// 代码管理Dto
    /// </summary>
    public class CodeManagementDto : FullBaseDto
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public int TotalFileCount { get; set; }
        public long TotalSize { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 创建代码管理输入Dto
    /// </summary>
    public class CreateCodeManagementInput
    {
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新代码管理输入Dto
    /// </summary>
    public class UpdateCodeManagementInput
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 更新项目信息输入Dto（仅更新项目名称、描述和备注）
    /// </summary>
    public class UpdateProjectInfoInput
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 代码管理分页查询输入Dto
    /// </summary>
    public class GetCodeManagementPagedInput : CommPagedInput
    {
        public string ProjectName { get; set; }
    }

    #endregion

    #region 代码片段相关DTO

    /// <summary>
    /// 代码片段Dto
    /// </summary>
    public class CodeSnippetDto
    {
        public Guid Id { get; set; }
        public Guid CodeManagementId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsFolder { get; set; }
        public string FileContent { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public int Level { get; set; }
        public int SortOrder { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<CodeSnippetDto> Children { get; set; }
    }

    /// <summary>
    /// 创建代码片段输入Dto
    /// </summary>
    public class CreateCodeSnippetInput
    {
        public Guid CodeManagementId { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public Guid? ParentId { get; set; }
        public bool IsFolder { get; set; }
        public string FileContent { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
        public int Level { get; set; }
    }

    /// <summary>
    /// 批量创建代码片段输入Dto
    /// </summary>
    public class BatchCreateCodeSnippetsInput
    {
        public Guid CodeManagementId { get; set; }
        public string ProjectName { get; set; }
        public List<CodeSnippetFileInfo> Files { get; set; }
    }

    /// <summary>
    /// 代码片段文件信息
    /// </summary>
    public class CodeSnippetFileInfo
    {
        public string FileName { get; set; }
        public string FilePath { get; set; }
        public string FileContent { get; set; }
        public long FileSize { get; set; }
        public string FileExtension { get; set; }
        public string FileType { get; set; }
    }

    /// <summary>
    /// 更新代码片段输入Dto
    /// </summary>
    public class UpdateCodeSnippetInput
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
    }

    /// <summary>
    /// 代码片段树形结构输出Dto
    /// </summary>
    public class CodeSnippetTreeOutput
    {
        public Guid CodeManagementId { get; set; }
        public string ProjectName { get; set; }
        public List<CodeSnippetDto> TreeData { get; set; }
    }

    /// <summary>
    /// 代码片段查询输入Dto
    /// </summary>
    public class GetCodeSnippetsInput
    {
        public Guid CodeManagementId { get; set; }
        public Guid? ParentId { get; set; }
        public bool? IsFolder { get; set; }
        public string FileName { get; set; }
    }

    /// <summary>
    /// 复制到新项目输入Dto
    /// </summary>
    public class CopyToNewProjectInput
    {
        public Guid SourceCodeManagementId { get; set; }
        public List<Guid> SelectedFileIds { get; set; }
        public string ProjectName { get; set; }
        public string Description { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 代码管理统计信息输出Dto
    /// </summary>
    public class CodeManagementStatisticsOutput
    {
        /// <summary>
        /// 项目总数
        /// </summary>
        public int TotalProjects { get; set; }

        /// <summary>
        /// 文件总数
        /// </summary>
        public int TotalFiles { get; set; }

        /// <summary>
        /// 总大小（字节）
        /// </summary>
        public long TotalSize { get; set; }
    }

    #endregion

    #region AI拆分识别模块相关DTO

    /// <summary>
    /// AI分析代码模块输入Dto
    /// </summary>
    public class AnalyzeCodeModulesInput
    {
        /// <summary>
        /// 代码管理ID
        /// </summary>
        public Guid CodeManagementId { get; set; }
    }

    /// <summary>
    /// AI分析代码模块输出Dto
    /// </summary>
    public class AnalyzeCodeModulesOutput
    {
        /// <summary>
        /// 模块列表
        /// </summary>
        public List<CodeModuleInfo> Modules { get; set; }
    }

    /// <summary>
    /// 代码模块信息
    /// </summary>
    public class CodeModuleInfo
    {
        /// <summary>
        /// 模块名称
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// 模块路径
        /// </summary>
        public string ModulePath { get; set; }

        /// <summary>
        /// 模块描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 文件ID列表
        /// </summary>
        public List<string> FileIds { get; set; }

        /// <summary>
        /// 文件路径列表
        /// </summary>
        public List<string> FilePaths { get; set; }

        /// <summary>
        /// 文件数量
        /// </summary>
        public int FileCount => FileIds?.Count ?? 0;
    }

    /// <summary>
    /// 批量拆分代码模块输入Dto
    /// </summary>
    public class BatchSplitModulesInput
    {
        /// <summary>
        /// 源代码管理ID
        /// </summary>
        public Guid SourceCodeManagementId { get; set; }

        /// <summary>
        /// 选中的模块列表
        /// </summary>
        public List<CodeModuleInfo> SelectedModules { get; set; }
    }

    /// <summary>
    /// 批量删除输入
    /// </summary>
    public class BatchDeleteInput
    {
        /// <summary>
        /// 要删除的ID列表
        /// </summary>
        public List<Guid> Ids { get; set; }
    }

    #endregion
}

