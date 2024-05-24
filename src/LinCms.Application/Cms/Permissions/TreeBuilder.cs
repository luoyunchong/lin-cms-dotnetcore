using System.Collections.Generic;
using System.Linq;

namespace LinCms.Cms.Permissions;

public record TreeBuilder
{
    public List<PermissionTreeNode> BuildPermissionTree(List<PermissionTreeNode> nodes)
    {
        var nodeDict = nodes.ToDictionary(n => n.Id);
        List<PermissionTreeNode> roots = new List<PermissionTreeNode>();

        foreach (var node in nodes)
        {
            if (node.ParentId == 0)  // 假定ParentId为0表示根节点
            {
                roots.Add(node);
            }
            else
            {
                if (nodeDict.TryGetValue(node.ParentId, out PermissionTreeNode parentNode))
                {
                    parentNode.Children.Add(node);
                }
            }
        }
        return roots;  // 返回森林中所有根节点的列表
    }

    public List<TreeNode> BuildTree(List<TreeNode> nodes)
    {
        var nodeDict = nodes.ToDictionary(n => n.Id);
        List<TreeNode> roots = new List<TreeNode>();

        foreach (var node in nodes)
        {
            if (node.ParentId == 0)  // 假定ParentId为0表示根节点
            {
                roots.Add(node);
            }
            else
            {
                if (nodeDict.TryGetValue(node.ParentId, out TreeNode parentNode))
                {
                    parentNode.Children.Add(node);
                }
            }
        }
        return roots;  // 返回森林中所有根节点的列表
    }
}
