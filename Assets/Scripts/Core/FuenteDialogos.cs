using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;

/// <summary>
/// Carga y aplica la tipografía Bitcount Single únicamente a los textos de diálogo de los maestros.
/// No modifica fuentes de minijuegos, menús ni otros elementos de interfaz.
/// </summary>
public static class FuenteDialogos
{
    const string RutaFontTTF = "Fonts/BitcountSingle-Regular";
    static TMP_FontAsset fuenteTMP;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reiniciar()
    {
        fuenteTMP = null;
    }

    public static TMP_FontAsset Obtener()
    {
        if (fuenteTMP == null)
        {
            Font fuente = Resources.Load<Font>(RutaFontTTF);
            if (fuente == null) return null;

            fuenteTMP = TMP_FontAsset.CreateFontAsset(
                fuente,
                90,
                9,
                GlyphRenderMode.SDFAA,
                1024,
                1024,
                AtlasPopulationMode.Dynamic
            );
            if (fuenteTMP != null)
            {
                fuenteTMP.name = "BitcountSingle-Regular SDF (Dialogos)";
                fuenteTMP.isMultiAtlasTexturesEnabled = true;
            }
        }
        return fuenteTMP;
    }

    public static void Aplicar(TMP_Text texto)
    {
        if (texto == null) return;

        TMP_FontAsset f = Obtener();
        if (f != null)
        {
            texto.font = f;
            if (f.material != null)
                texto.fontSharedMaterial = f.material;
            texto.UpdateMeshPadding();
            texto.ForceMeshUpdate();
        }
    }

    public static void AplicarEnPanel(GameObject panel)
    {
        if (panel == null) return;
        TMP_Text[] textos = panel.GetComponentsInChildren<TMP_Text>(true);
        foreach (TMP_Text t in textos)
            Aplicar(t);
    }
}
