using System;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;
using PlanVisualizerArchitecture.AnalyzerPack.VisualizerRenderPack.ObjectCreatePack;

namespace PlanVisualizerArchitecture.AnalyzerPack.ScriptReaderPack.PlanReaderPack {
   /// <summary>The script reader component that parse "Plan JSON" data into objects</summary>
   public static class PlanReaderInterface {

      /// <summary>Read data from the string content and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argPlanData">The JSON data pf plan</param>
      public static void ParseData(ScriptInfoItem argScriptInfoItem, string argPlanData) {
         throw new NotImplementedException();
      }

   }
}