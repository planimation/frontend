using System;
using PlanVisualizerArchitecture.Entity.ScriptInfoPack;

namespace PlanVisualizerArchitecture.AnalyzerPack.ScriptReaderPack.AnimationProfileReaderPack {
   /// <summary>The script reader component that parse "AnimationProfile" data into objects</summary>
   public static class AnimationProfileReaderInterface {

      /// <summary>Read data from the string content and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argAniProfileData">The content of AnimationProfile data</param>
      public static void ParseDataFromContent(ScriptInfoItem argScriptInfoItem, string argAniProfileData) {
         throw new NotImplementedException();
      }

      /// <summary>Read data from given file and update the data of ScriptInfoItem</summary>
      /// <param name="argScriptInfoItem">The ScriptInfoItem to store data</param>
      /// <param name="argFileAddress">The file address of AnimationProfile data</param>
      public static void ParseDataFromFile(ScriptInfoItem argScriptInfoItem, string argFileAddress) {
         throw new NotImplementedException();
      }

   }
}