using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wpf.Frame.Models;

namespace Wpf.Frame.ViewModels
{
  public  class MainViewModel
    {
        //菜单  集合
        public List<MeunItemModel> TreeList { get; set; }

        public MainViewModel() { 
        
        TreeList= new List<MeunItemModel>();
            { 

            MeunItemModel  tim=new MeunItemModel();
            tim.Header = "工艺设计";//&#xe601;
            tim.IconCode = "\xe629";//&#xe602;

              

                tim.Children.Add(new MeunItemModel
            {
                Header = "加工工艺",
                

            });
            tim.Children.Add(new MeunItemModel
            {
                Header = "加工工艺1",


            });
            tim.Children.Add(new MeunItemModel
            {
                Header = "加工工艺2",


            });
            tim.Children.Add(new MeunItemModel
            {
                Header = "加工工艺3",


            });
            tim.Children.Add(new MeunItemModel
            {
                Header = "加工工艺4",


            });
            TreeList.Add(tim);

                MeunItemModel subMenu=new MeunItemModel();
                subMenu.Header = "二级菜单";
                subMenu.Children.Add(new MeunItemModel { Header="三级菜单"});
                tim.Children.Add(subMenu);
            }

        }  
    }
}
