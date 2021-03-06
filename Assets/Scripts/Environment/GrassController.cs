using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassController : MonoBehaviour
{
    const int maxDensity = 128;

    void Awake()
    {
        Reset();
    }

    public void Reset()
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // create an appropriately sized patch
        var cut = new int[data.detailWidth, data.detailHeight];

        // clear layers that contain cut data
        data.SetDetailLayer(0, 0, 1, cut);
        data.SetDetailLayer(0, 0, 2, cut);
        data.SetDetailLayer(0, 0, 3, cut);

        // set fully grown grass
        for (var h = 0; h < data.detailHeight; h++)
            for (var w = 0; w < data.detailHeight; w++)
                cut[w, h] = maxDensity;
        data.SetDetailLayer(0, 0, 0, cut);
    }

    public void Grow()
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        var cut0 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 0);
        var cut1 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 1);
        for (var h = 0; h < data.detailHeight; h++)
            for (var w = 0; w < data.detailHeight; w++)
                if (cut1[w, h] > 0)
                    cut0[w, h] = cut1[w, h];
        data.SetDetailLayer(0, 0, 0, cut0);

        var cut2 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 2);
        data.SetDetailLayer(0, 0, 1, cut2);

        var cut3 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 3);
        data.SetDetailLayer(0, 0, 2, cut3);

        data.SetDetailLayer(0, 0, 3, new int[data.detailWidth, data.detailHeight]);
    }

    public void Cut(Vector3 at, float radius)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // calculate detail scale
        float r = (float)data.detailResolution;
        Vector3 detailScale = new Vector3(r / data.size.x, r / data.size.y, r / data.size.z);

        // calculate center in terrain detail coordinates
        var center = at - terrain.transform.position;
        center.Scale(detailScale);

        // radius in a particular dimension
        var wr = Mathf.Round(radius * detailScale.x);
        var hr = Mathf.Round(radius * detailScale.z);

        // calculate rectangle bounds
        int w0 = (int)Mathf.Floor(center.x - wr);
        int w1 = (int)Mathf.Round(center.x);
        int w2 = (int)Mathf.Floor(center.x + wr);

        int h0 = (int)Mathf.Floor(center.z - hr);
        int h1 = (int)Mathf.Round(center.z);
        int h2 = (int)Mathf.Floor(center.z + hr);

        // clamp bounds to avoid overflow
        w0 = Mathf.Clamp(w0, 0, data.detailWidth);
        w1 = Mathf.Clamp(w1, 0, data.detailWidth);
        w2 = Mathf.Clamp(w2, 0, data.detailWidth);

        h0 = Mathf.Clamp(h0, 0, data.detailHeight);
        h1 = Mathf.Clamp(h1, 0, data.detailHeight);
        h2 = Mathf.Clamp(h2, 0, data.detailHeight);

        // get the patch
        var cut0 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 0);
        var cut1 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 1);
        var cut2 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 2);
        var cut3 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 3);

        // offset center to patch coordinates
        w1 -= w0;
        h1 -= h0;

        // cache bounds
        var wn = cut0.GetLength(0);
        var hn = cut0.GetLength(1);

        wr = Mathf.Floor(radius * detailScale.x);
        hr = Mathf.Floor(radius * detailScale.z);

        // calculate inverse ellipse dimensions
        var wrinv = 1 / wr;
        var hrinv = 1 / hr;

        for (var h = 0; h < hn; h++)
        {
            var dh = (h - h1) * hrinv;
            for (var w = 0; w < wn; w++)
            {
                var dw = (w - w1) * wrinv;
                // ignore everything outside of the circle
                if (dh * dh + dw * dw > 1) continue;

                // cut the detail layer
                if (cut2[w, h] > 0)
                    cut3[w, h] = maxDensity;
                cut2[w, h] = cut1[w, h];
                cut1[w, h] = cut0[w, h];
                cut0[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(w0, h0, 0, cut0);
        data.SetDetailLayer(w0, h0, 1, cut1);
        data.SetDetailLayer(w0, h0, 2, cut2);
        data.SetDetailLayer(w0, h0, 3, cut3);
    }

    public int CutCounted(Vector3 at, float radius)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return 0;
        }

        var data = terrain.terrainData;

        // calculate detail scale
        float r = (float)data.detailResolution;
        Vector3 detailScale = new Vector3(r / data.size.x, r / data.size.y, r / data.size.z);

        // calculate center in terrain detail coordinates
        var center = at - terrain.transform.position;
        center.Scale(detailScale);

        // radius in a particular dimension
        var wr = Mathf.Round(radius * detailScale.x);
        var hr = Mathf.Round(radius * detailScale.z);

        // calculate rectangle bounds
        int w0 = (int)Mathf.Floor(center.x - wr);
        int w1 = (int)Mathf.Round(center.x);
        int w2 = (int)Mathf.Floor(center.x + wr);

        int h0 = (int)Mathf.Floor(center.z - hr);
        int h1 = (int)Mathf.Round(center.z);
        int h2 = (int)Mathf.Floor(center.z + hr);

        // clamp bounds to avoid overflow
        w0 = Mathf.Clamp(w0, 0, data.detailWidth);
        w1 = Mathf.Clamp(w1, 0, data.detailWidth);
        w2 = Mathf.Clamp(w2, 0, data.detailWidth);

        h0 = Mathf.Clamp(h0, 0, data.detailHeight);
        h1 = Mathf.Clamp(h1, 0, data.detailHeight);
        h2 = Mathf.Clamp(h2, 0, data.detailHeight);

        // get the patch
        var cut0 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 0);
        var cut1 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 1);
        var cut2 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 2);
        var cut3 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 3);

        // offset center to patch coordinates
        w1 -= w0;
        h1 -= h0;

        // cache bounds
        var wn = cut0.GetLength(0);
        var hn = cut0.GetLength(1);

        wr = Mathf.Floor(radius * detailScale.x);
        hr = Mathf.Floor(radius * detailScale.z);

        // calculate inverse ellipse dimensions
        var wrinv = 1 / wr;
        var hrinv = 1 / hr;

        var count = 0;

        for (var h = 0; h < hn; h++)
        {
            var dh = (h - h1) * hrinv;
            for (var w = 0; w < wn; w++)
            {
                var dw = (w - w1) * wrinv;
                // ignore everything outside of the circle
                if (dh * dh + dw * dw > 1) continue;

                count += cut2[w, h];
                count += cut1[w, h];
                count += cut0[w, h];

                // cut the detail layer
                if (cut2[w, h] > 0)
                    cut3[w, h] = maxDensity;
                cut2[w, h] = cut1[w, h];
                cut1[w, h] = cut0[w, h];
                cut0[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(w0, h0, 0, cut0);
        data.SetDetailLayer(w0, h0, 1, cut1);
        data.SetDetailLayer(w0, h0, 2, cut2);
        data.SetDetailLayer(w0, h0, 3, cut3);

        return count;
    }

    public void CutArc(Vector3 at, float radius, Vector3 cutDirection, float arcAngle)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // calculate detail scale
        float r = (float)data.detailResolution;
        Vector3 detailScale = new Vector3(r / data.size.x, r / data.size.y, r / data.size.z);

        // calculate center in terrain detail coordinates
        var center = at - terrain.transform.position;
        center.Scale(detailScale);

        // radius in a particular dimension
        var wr = Mathf.Round(radius * detailScale.x);
        var hr = Mathf.Round(radius * detailScale.z);

        // calculate rectangle bounds
        int w0 = (int)Mathf.Floor(center.x - wr);
        int w1 = (int)Mathf.Round(center.x);
        int w2 = (int)Mathf.Floor(center.x + wr);

        int h0 = (int)Mathf.Floor(center.z - hr);
        int h1 = (int)Mathf.Round(center.z);
        int h2 = (int)Mathf.Floor(center.z + hr);

        // clamp bounds to avoid overflow
        w0 = Mathf.Clamp(w0, 0, data.detailWidth);
        w1 = Mathf.Clamp(w1, 0, data.detailWidth);
        w2 = Mathf.Clamp(w2, 0, data.detailWidth);

        h0 = Mathf.Clamp(h0, 0, data.detailHeight);
        h1 = Mathf.Clamp(h1, 0, data.detailHeight);
        h2 = Mathf.Clamp(h2, 0, data.detailHeight);

        // get the patch
        var cut0 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 0);
        var cut1 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 1);
        var cut2 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 2);
        var cut3 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 3);

        // offset center to patch coordinates
        w1 -= w0;
        h1 -= h0;

        // cache bounds
        var wn = cut0.GetLength(0);
        var hn = cut0.GetLength(1);

        // calculate inverse ellipse dimensions
        var wrinv = 1 / wr;
        var hrinv = 1 / hr;

        var direction = new Vector2(cutDirection.x, cutDirection.z);

        for (var h = 0; h < hn; h++)
        {
            var dh = (h - h1) * hrinv;
            for (var w = 0; w < wn; w++)
            {
                var dw = (w - w1) * wrinv;
                // ignore everything outside of the circle
                if (dh * dh + dw * dw > 1) continue;

                var delta = new Vector2(dh, dw).normalized;
                if (Mathf.Abs(Vector2.Angle(delta, direction)) > arcAngle)
                    continue;

                // cut the detail layer
                if (cut2[w, h] > 0)
                    cut3[w, h] = maxDensity;
                cut2[w, h] = cut1[w, h];
                cut1[w, h] = cut0[w, h];
                cut0[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(w0, h0, 0, cut0);
        data.SetDetailLayer(w0, h0, 1, cut1);
        data.SetDetailLayer(w0, h0, 2, cut2);
        data.SetDetailLayer(w0, h0, 3, cut3);
    }

    public void Plant(Vector3 at, float radius)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // calculate detail scale
        float r = (float)data.detailResolution;
        Vector3 detailScale = new Vector3(r / data.size.x, r / data.size.y, r / data.size.z);

        // calculate center in terrain detail coordinates
        var center = at - terrain.transform.position;
        center.Scale(detailScale);

        // radius in a particular dimension
        var wr = Mathf.Round(radius * detailScale.x);
        var hr = Mathf.Round(radius * detailScale.z);

        // calculate rectangle bounds
        int w0 = (int)Mathf.Floor(center.x - wr);
        int w1 = (int)Mathf.Round(center.x);
        int w2 = (int)Mathf.Floor(center.x + wr);

        int h0 = (int)Mathf.Floor(center.z - hr);
        int h1 = (int)Mathf.Round(center.z);
        int h2 = (int)Mathf.Floor(center.z + hr);

        // clamp bounds to avoid overflow
        w0 = Mathf.Clamp(w0, 0, data.detailWidth);
        w1 = Mathf.Clamp(w1, 0, data.detailWidth);
        w2 = Mathf.Clamp(w2, 0, data.detailWidth);

        h0 = Mathf.Clamp(h0, 0, data.detailHeight);
        h1 = Mathf.Clamp(h1, 0, data.detailHeight);
        h2 = Mathf.Clamp(h2, 0, data.detailHeight);

        // get the patch
        var cut0 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 0);
        var cut1 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 1);
        var cut2 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 2);
        var cut3 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 3);

        // offset center to patch coordinates
        w1 -= w0;
        h1 -= h0;

        // cache bounds
        var wn = cut0.GetLength(0);
        var hn = cut0.GetLength(1);

        // calculate inverse ellipse dimensions
        var wrinv = 1 / wr;
        var hrinv = 1 / hr;

        for (var h = 0; h < hn; h++)
        {
            var dh = (h - h1) * hrinv;
            for (var w = 0; w < wn; w++)
            {
                var dw = (w - w1) * wrinv;
                // ignore everything outside of the circle
                if (dh * dh + dw * dw > 1) continue;

                if (cut1[w, h] > 0)
                    cut0[w, h] = cut1[w, h];
                cut1[w, h] = cut2[w, h];
                cut2[w, h] = cut3[w, h];
                cut3[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(w0, h0, 0, cut0);
        data.SetDetailLayer(w0, h0, 1, cut1);
        data.SetDetailLayer(w0, h0, 2, cut2);
        data.SetDetailLayer(w0, h0, 3, cut3);
    }

    public void GrowPatchy(float patchDensity)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // get the patch
        var cut0 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 0);
        var cut1 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 1);
        var cut2 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 2);
        var cut3 = data.GetDetailLayer(0, 0, data.detailWidth, data.detailHeight, 3);

        for (var h = 0; h < data.detailHeight; h++)
        {
            for (var w = 0; w < data.detailWidth; w++)
            {
                if (Random.Range(0f, 1f) < patchDensity) continue;

                if (cut1[w, h] > 0)
                    cut0[w, h] = cut1[w, h];
                cut1[w, h] = cut2[w, h];
                cut2[w, h] = cut3[w, h];
                cut3[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(0, 0, 0, cut0);
        data.SetDetailLayer(0, 0, 1, cut1);
        data.SetDetailLayer(0, 0, 2, cut2);
        data.SetDetailLayer(0, 0, 3, cut3);
    }

    public void PlantPatchy(Vector3 at, float radius, float patchDensity)
    {
        var terrain = GetComponent<Terrain>();
        if (terrain == null)
        {
            Debug.LogWarning("Terrain component missing.", gameObject);
            return;
        }

        var data = terrain.terrainData;

        // calculate detail scale
        float r = (float)data.detailResolution;
        Vector3 detailScale = new Vector3(r / data.size.x, r / data.size.y, r / data.size.z);

        // calculate center in terrain detail coordinates
        var center = at - terrain.transform.position;
        center.Scale(detailScale);

        // radius in a particular dimension
        var wr = Mathf.Round(radius * detailScale.x);
        var hr = Mathf.Round(radius * detailScale.z);

        // calculate rectangle bounds
        int w0 = (int)Mathf.Floor(center.x - wr);
        int w1 = (int)Mathf.Round(center.x);
        int w2 = (int)Mathf.Floor(center.x + wr);

        int h0 = (int)Mathf.Floor(center.z - hr);
        int h1 = (int)Mathf.Round(center.z);
        int h2 = (int)Mathf.Floor(center.z + hr);

        // clamp bounds to avoid overflow
        w0 = Mathf.Clamp(w0, 0, data.detailWidth);
        w1 = Mathf.Clamp(w1, 0, data.detailWidth);
        w2 = Mathf.Clamp(w2, 0, data.detailWidth);

        h0 = Mathf.Clamp(h0, 0, data.detailHeight);
        h1 = Mathf.Clamp(h1, 0, data.detailHeight);
        h2 = Mathf.Clamp(h2, 0, data.detailHeight);

        // get the patch
        var cut0 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 0);
        var cut1 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 1);
        var cut2 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 2);
        var cut3 = data.GetDetailLayer(w0, h0, w2 - w0, h2 - h0, 3);

        // offset center to patch coordinates
        w1 -= w0;
        h1 -= h0;

        // cache bounds
        var wn = cut0.GetLength(0);
        var hn = cut0.GetLength(1);

        // calculate inverse ellipse dimensions
        var wrinv = 1 / wr;
        var hrinv = 1 / hr;

        for (var h = 0; h < hn; h++)
        {
            var dh = (h - h1) * hrinv;
            for (var w = 0; w < wn; w++)
            {
                var dw = (w - w1) * wrinv;
                // ignore everything outside of the circle
                if (dh * dh + dw * dw > 1) continue;
                if (Random.Range(0, 1) < patchDensity) continue;

                if (cut1[w, h] > 0)
                    cut0[w, h] = cut1[w, h];
                cut1[w, h] = cut2[w, h];
                cut2[w, h] = cut3[w, h];
                cut3[w, h] = 0;
            }
        }

        // update the patch
        data.SetDetailLayer(w0, h0, 0, cut0);
        data.SetDetailLayer(w0, h0, 1, cut1);
        data.SetDetailLayer(w0, h0, 2, cut2);
        data.SetDetailLayer(w0, h0, 3, cut3);
    }
}
