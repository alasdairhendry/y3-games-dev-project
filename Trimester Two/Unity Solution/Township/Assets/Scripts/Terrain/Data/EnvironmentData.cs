using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class EnvironmentData : UpdatableScriptableObject {

    public EnvironmentDetailData[] detailData;

    public List<DetailGroup> detailGroups;

    [System.Serializable]
    public class Detail
    {
        public EnvironmentDetailData data;
        public float ratio;
    }

    [System.Serializable]
    public class DetailGroup
    {
        public string GroupName = "";
        public string BiomeName = "";
        [Range(0.1f, 2.0f)] public float overallModifier = 1.0f;
        public List<Detail> group = new List<Detail> ();
    }
}
