﻿using DevExpress.ExpressApp;
using Xpand.XAF.Modules.Reactive.Logger;
using Xpand.XAF.Modules.Reactive.Logger.Hub;

namespace Xpand.XAF.Modules.Reactive.Tests{
    public class TestModule:ModuleBase{
        public TestModule(){
            RequiredModuleTypes.Add(typeof(ReactiveModule));
            ReactiveModuleBase.Unload(typeof(ReactiveLoggerHubModule), typeof(ReactiveLoggerModule),
                typeof(ReactiveModule));
        }
    }
}