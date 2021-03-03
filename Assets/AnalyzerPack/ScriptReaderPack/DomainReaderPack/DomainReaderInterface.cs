using System;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;

namespace PlanVisualizerArchitecture.AnalyzerPack.ScriptReaderPack.DomainReaderPack {
   /// <summary>The script reader component that parse "Domain PDDL" data into objects</summary>
   public static class DomainReaderInterface {

      /// <summary>Read data from the string content and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argDomainData">The content of Domain data</param>
      public static void ParseDataFromContent(ScriptInfoItem argScriptInfoItem, string argDomainData) {
         throw new NotImplementedException();
      }

      /// <summary>Read data from given file and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argFileAddress">The file address of Domain data</param>
      public static void ParseDataFromFile(ScriptInfoItem argScriptInfoItem, string argFileAddress) {
         throw new NotImplementedException();
      }

   }
}