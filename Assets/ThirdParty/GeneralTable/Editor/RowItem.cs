//-----------------------------------------------------------------------
// <filename>TableDataSource</fileName>
// <copyright>
//     Copyright (c) 2018 Zhang Hui. All rights reserved.
// </copyright>
// <describe> ## </describe>
// <email> whdhxyzh@gmail.com </email>
// <time> #2018/12/18 星期二 14:58:49# </time>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;

namespace GameFramework.Taurus
{
    public class RowItem<T>:TreeViewItem
    {
        public T Data;

        public RowItem(int _id,T data)
        {
            id = _id;
            Data = data;
        }
    }
}
