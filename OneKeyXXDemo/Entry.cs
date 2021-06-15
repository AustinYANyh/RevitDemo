/*============================================================================================
   *   Copyright (C) 2021 All rights reserved.
   *   
   *   文件名称：Entry.cs
   *   创 建 者：yanyunhao
   *   创建日期：2021/6/15 17:10:54
   *   描    述：
==============================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;

namespace OneKeyXXTest
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class Entry : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            OneKeyXXExternalJobS onekey = new OneKeyXXExternalJobS();
            onekey.DoJob(commandData.Application);

            return Result.Succeeded;
        }
    }
}

