using System;
using Agenda.Core;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Agenda.Modules.SimpleModule;

public class SimpleViewModel : BasicViewModel
{
    public SimpleViewModel(Connection conn) : base(conn)
    {
    }

    public void Test()
    {
        if (this.Conn.Driver is SimpleDriver driver)
        {
            Console.WriteLine(driver.Test());
        }
    }
}