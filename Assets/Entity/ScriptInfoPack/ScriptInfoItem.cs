namespace PlanVisualizerArchitecture.Entity.ScriptInfoPack {
   public sealed class ScriptInfoItem {

      // Store the raw file data.
      // They could be useless, but it won't hurt to keep a copy just in case.
      public string RawData_domain;
      public string RawData_problem;
      public string RawData_plan;
      public string RawData_animationProfile;

      // The objects in this project
      public Script_ObjectItem[] objectInfos;
      // The predicates in this project
      public Script_PredicateItem[] predicateInfos;

   }
}