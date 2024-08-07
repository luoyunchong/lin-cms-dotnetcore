﻿using System;
using LinCms.Data;

namespace LinCms.Blog.Articles;

public class ArticleSearchDto : PageDto
{
    /// <summary>
    /// 分类Id
    /// </summary>
    public Guid? ClassifyId { get; set; }
    public Guid? ChannelId { get; set; }
    public Guid? TagId { get; set; }
    public string Title { get; set; }
    public Guid? ArticleId { get; set; }
    public long? UserId { get; set; }
   
    /// <summary>
    /// 收藏集合Id
    /// </summary>
    public Guid? CollectionId { get; set; }

    public ArticleSearchTypeEnum? ArticleSearchType { get; set; }
    public override string ToString()
    {
        return $"{ClassifyId}:{ChannelId}:{TagId}:{Title}:{UserId}:{Count}:{Page}:{Sort}";
    }

}


public enum ArticleSearchTypeEnum
{
    /// <summary>
    /// 点赞
    /// </summary>
    Like,
}