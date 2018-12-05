﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;

namespace MyAOPApplication1
{
    public class User
    {
        public string Name { set; get; }
        public string PassWord { set; get; }
    }

    
    //RealProxy
    public class MyRealProxy<T>:RealProxy
    {
        private T _target;
        public MyRealProxy(T target) : base(typeof(T))
        {
            this._target = target;
        }
       public override IMessage Invoke(IMessage msg)
       {
            PreProceede(msg);
            IMethodCallMessage callMessage = (IMethodCallMessage)msg;
            object returnValue = callMessage.MethodBase.Invoke(this._target, callMessage.Args);
            PostProceede(msg);
            return new ReturnMessage(returnValue, new object[0], 0, null, callMessage);
        }
       public void PreProceede(IMessage msg)
       {
           Console.WriteLine("方法执行前");
       }
       public void PostProceede(IMessage msg)
       {
           Console.WriteLine("方法执行后");
       }
    }
   //TransparentProxy
   public static class TransparentProxy
   {
        public static T Create<T>()
        {
           T instance = Activator.CreateInstance<T>();
           MyRealProxy<T> realProxy = new MyRealProxy<T>(instance);
           T transparentProxy = (T)realProxy.GetTransparentProxy();
           return transparentProxy;
        }
   }

   public interface IUserProcessor
   {
       void RegUser(User user);
   }

   public class UserProcessor : MarshalByRefObject, IUserProcessor
   {
       public void RegUser(User user)
       {
           Console.WriteLine("用户已注册。");
       }
   }
}
