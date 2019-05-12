using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Dirtiness : MonoBehaviour {

    public ParticleSystem Dust;
    public List<GameObject> dirtyObjects;

    public string dirtinessParameterName;

    public int defaultValue;
    private int dirtiness;

    private int[] particleRate = { 0, 1000, 10000 };

    void Start()
    {
        SessionParameters parameters = FindObjectOfType<SessionParameters>();
        if (parameters != null)
        {
            dirtiness = parameters.GetChoice(dirtinessParameterName, defaultValue).Index;
        }
        else
        {
            dirtiness = defaultValue;
        }

        for (int i = 0; i < dirtyObjects.Count; i++)
        {
            if (dirtyObjects[i] != null)
                dirtyObjects[i].SetActive(i <= dirtiness);
        }

        var em = Dust.emission;
        var rate = new ParticleSystem.MinMaxCurve();
        rate.constantMax = particleRate[dirtiness < particleRate.Length ? dirtiness : particleRate.Length - 1];
        em.rate = rate;
    }
}
