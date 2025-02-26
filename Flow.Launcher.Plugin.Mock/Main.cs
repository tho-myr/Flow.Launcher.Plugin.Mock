using System.Collections.Generic;
using System.Linq;

namespace Flow.Launcher.Plugin.Mock {
	
    public class Main : IPlugin {
		
        private PluginInitContext _context;

        public void Init(PluginInitContext context) {
            _context = context;
        }

        public List<Result> Query(Query query) {
            return new List<Result> {
                new Result {
                    Title = "🎉 initial mock search",
                    SubTitle = "CrEaTe YoUr MoCk HeRe",
                    IcoPath = "Images/icon.png"
                }
            };
        }

    }
}