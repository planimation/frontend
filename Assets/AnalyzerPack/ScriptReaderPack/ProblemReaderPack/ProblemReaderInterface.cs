using System;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;

namespace PlanVisualizerArchitecture.AnalyzerPack.ScriptReaderPack.ProblemReaderPack {
   /// <summary>The script reader component that parse "Problem PDDL" data into objects</summary>
   public static class ProblemReaderInterface {

      /// <summary>Read data from the string content and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argProblemData">The content of Problem data</param>
      public static void ParseDataFromContent(ScriptInfoItem argScriptInfoItem, string argProblemData) {
         throw new NotImplementedException();
      }

      /// <summary>Read data from given file and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argFileAddress">The file address of Problem data</param>
      public static void ParseDataFromFile(ScriptInfoItem argScriptInfoItem, string argFileAddress) {
         throw new NotImplementedException();
      }

   }
}