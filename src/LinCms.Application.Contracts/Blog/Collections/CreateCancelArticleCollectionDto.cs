using System;

namespace LinCms.Blog.Collections;

public class CreateCancelArticleCollectionDto
{
    public Guid ArticleId { get; set; }
    public Guid? CollectionId { get; set; }
}