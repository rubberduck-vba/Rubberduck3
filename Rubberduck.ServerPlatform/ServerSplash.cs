using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.ServerPlatform
{
    public static class ServerSplash
    {
        public static string GetRenderString(AssemblyName serverAppAssemblyName)
        {
            // https://ascii-generator.site/

            var banner = @"                                                                                
                  .::::.                                                        
               -+*++==++*=:                                                     
             -*+---------=*+.                                                   
            :*+------=++---+**+=++.                                             
            :*=-------+=--=*******:                                             
  :-.       .*+----------=******=.                                    ...       
  ***+-.     :*+----------==--:      -++++++==:    =++++++=:       +@@@@@@#:    
 =*--=+**=-:...+*=--------==.        +#@@%##%%@@-  +@@@##%%@@*.   .##+-:-*@@:   
 **------==+++++++---------+*=         @@+:...+@@:  %@%-...:%@%:      ..:#@%-.  
.*+--------------------------**.       @@#+=+#@@*-  %@#:    .@@+.    -@@@@@+:   
:*=---------------------------+*       @@%%%@@@=:   %@#:    .@@+.      :-*@@=   
.*+----------------------------*-      @@+:.:+@@+   %@#:   :#@%-.         %@#:  
 =*=--------------------------=*:    #%@@@%=  .%@@##@@@@%%@@@#=:  @@%###%@@%=:  
  +*=-------------------------**     :=++++-.   -====++++++=::     -+****+=:.   
   -*+----------------------+*+                                                 
    .=*+=---------------==+*=.                                                  
       :=+**++++===+++**+-.                                                     
            .::::::::.                                                          
                                                                                
";
            return $"{banner}\n{serverAppAssemblyName.Name} v{serverAppAssemblyName.Version?.ToString(3) ?? "0"}";
        }
    }
}
