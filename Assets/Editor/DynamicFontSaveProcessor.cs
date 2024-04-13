using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

public class DynamicFontSaveProcessor : AssetModificationProcessor
{
    private const string MENU_NAME_ALLOW_SAVING_DYNAMIC_FONTS = "DMW Tools/Utils/Allow saving dynamic fonts";
    private const string ALLOW_SAVING_DYNAMIC_FONTS_PREF = "AllowSavingDynamicFonts";
 
    private static string[] OnWillSaveAssets(string[] paths)
    {
        return EditorPrefs.GetBool(ALLOW_SAVING_DYNAMIC_FONTS_PREF) ? paths : paths.Where(path => !IsDynamicFontAsset(path)).ToArray();
    }
 
    private static bool IsDynamicFontAsset(string assetPath)
    {
        var assetType = AssetDatabase.GetMainAssetTypeAtPath(assetPath);
        if (!typeof(TMP_FontAsset).IsAssignableFrom(assetType)) return false;
 
        var fontAsset = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>(assetPath);
        if (fontAsset == null) return false;
 
        return fontAsset.atlasPopulationMode == AtlasPopulationMode.Dynamic;

    }
 
    [MenuItem(MENU_NAME_ALLOW_SAVING_DYNAMIC_FONTS)]
    private static void MenuAllowSavingDynamicFonts()
    {
        EditorPrefs.SetBool(ALLOW_SAVING_DYNAMIC_FONTS_PREF, !EditorPrefs.GetBool(ALLOW_SAVING_DYNAMIC_FONTS_PREF));
    }
 
    [MenuItem(MENU_NAME_ALLOW_SAVING_DYNAMIC_FONTS, isValidateFunction: true)]
    private static bool MenuAllowSavingDynamicFontsValidate()
    {
        Menu.SetChecked(MENU_NAME_ALLOW_SAVING_DYNAMIC_FONTS, EditorPrefs.GetBool(ALLOW_SAVING_DYNAMIC_FONTS_PREF));
        return true;
    }
}
