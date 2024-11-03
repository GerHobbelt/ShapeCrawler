﻿using System.Collections.Generic;

namespace ShapeCrawler.Tables;

/// <summary>
///     Represents the tablestyle of a table.
/// </summary>
public interface ITableStyle
{
    /// <summary>
    ///     Gets the name of the style.
    /// </summary> 
    public string Name { get; }

    /// <summary>
    ///     Gets the GUID of the style.
    /// </summary>
    public string GUID { get; }
}

internal class TableStyle : ITableStyle
{
    public TableStyle(string name)
    {
        this.Name = name;
    }

    public static ITableStyle NoStyleNoGrid => new TableStyle("No Style, No Grid") { GUID = "{2D5ABB26-0587-4C30-8999-92F81FD0307C}" };

    public static ITableStyle NoStyleTableGrid => new TableStyle("No Style, Table Grid") { GUID = "{5940675A-B579-460E-94D1-54222C63F5DA}" };

    public static ITableStyle ThemedStyle1Accent1 => new TableStyle("Themed Style 1 - Accent 1") { GUID = "{3C2FFA5D-87B4-456A-9821-1D502468CF0F}" };

    public static ITableStyle ThemedStyle1Accent2 => new TableStyle("Themed Style 1 - Accent 2") { GUID = "{284E427A-3D55-4303-BF80-6455036E1DE7}" };

    public static ITableStyle ThemedStyle1Accent3 => new TableStyle("Themed Style 1 - Accent 3") { GUID = "{69C7853C-536D-4A76-A0AE-DD22124D55A5}" };

    public static ITableStyle ThemedStyle1Accent4 => new TableStyle("Themed Style 1 - Accent 4") { GUID = "{775DCB02-9BB8-47FD-8907-85C794F793BA}" };

    public static ITableStyle ThemedStyle1Accent5 => new TableStyle("Themed Style 1 - Accent 5") { GUID = "{35758FB7-9AC5-4552-8A53-C91805E547FA}" };

    public static ITableStyle ThemedStyle1Accent6 => new TableStyle("Themed Style 1 - Accent 6") { GUID = "{08FB837D-C827-4EFA-A057-4D05807E0F7C}" };

    public static ITableStyle ThemedStyle2Accent1 => new TableStyle("Themed Style 2 - Accent 1") { GUID = "{D113A9D2-9D6B-4929-AA2D-F23B5EE8CBE7}" };

    public static ITableStyle ThemedStyle2Accent2 => new TableStyle("Themed Style 2 - Accent 2") { GUID = "{18603FDC-E32A-4AB5-989C-0864C3EAD2B8}" };

    public static ITableStyle ThemedStyle2Accent3 => new TableStyle("Themed Style 2 - Accent 3") { GUID = "{306799F8-075E-4A3A-A7F6-7FBC6576F1A4}" };

    public static ITableStyle ThemedStyle2Accent4 => new TableStyle("Themed Style 2 - Accent 4") { GUID = "{E269D01E-BC32-4049-B463-5C60D7B0CCD2}" };

    public static ITableStyle ThemedStyle2Accent5 => new TableStyle("Themed Style 2 - Accent 5") { GUID = "{327F97BB-C833-4FB7-BDE5-3F7075034690}" };

    public static ITableStyle ThemedStyle2Accent6 => new TableStyle("Themed Style 2 - Accent 6") { GUID = "{638B1855-1B75-4FBE-930C-398BA8C253C6}" };

    public static ITableStyle LightStyle1 => new TableStyle("Light Style 1") { GUID = "{9D7B26C5-4107-4FEC-AEDC-1716B250A1EF}" };

    public static ITableStyle LightStyle1Accent1 => new TableStyle("Light Style 1 - Accent 1") { GUID = "{3B4B98B0-60AC-42C2-AFA5-B58CD77FA1E5}" };

    public static ITableStyle LightStyle1Accent2 => new TableStyle("Light Style 1 - Accent 2") { GUID = "{0E3FDE45-AF77-4B5C-9715-49D594BDF05E}" };

    public static ITableStyle LightStyle1Accent3 => new TableStyle("Light Style 1 - Accent 3") { GUID = "{C083E6E3-FA7D-4D7B-A595-EF9225AFEA82}" };

    public static ITableStyle LightStyle1Accent4 => new TableStyle("Light Style 1 - Accent 4") { GUID = "{D27102A9-8310-4765-A935-A1911B00CA55}" };

    public static ITableStyle LightStyle1Accent5 => new TableStyle("Light Style 1 - Accent 5") { GUID = "{5FD0F851-EC5A-4D38-B0AD-8093EC10F338}" };

    public static ITableStyle LightStyle1Accent6 => new TableStyle("Light Style 1 - Accent 6") { GUID = "{68D230F3-CF80-4859-8CE7-A43EE81993B5}" };

    public static ITableStyle LightStyle2 => new TableStyle("Light Style 2") { GUID = "{7E9639D4-E3E2-4D34-9284-5A2195B3D0D7}" };

    public static ITableStyle LightStyle2Accent1 => new TableStyle("Light Style 2 - Accent 1") { GUID = "{69012ECD-51FC-41F1-AA8D-1B2483CD663E}" };

    public static ITableStyle LightStyle2Accent2 => new TableStyle("Light Style 2 - Accent 2") { GUID = "{72833802-FEF1-4C79-8D5D-14CF1EAF98D9}" };

    public static ITableStyle LightStyle2Accent3 => new TableStyle("Light Style 2 - Accent 3") { GUID = "{F2DE63D5-997A-4646-A377-4702673A728D}" };

    public static ITableStyle LightStyle2Accent4 => new TableStyle("Light Style 2 - Accent 4") { GUID = "{17292A2E-F333-43FB-9621-5CBBE7FDCDCB}" };

    public static ITableStyle LightStyle2Accent5 => new TableStyle("Light Style 2 - Accent 5") { GUID = "{5A111915-BE36-4E01-A7E5-04B1672EAD32}" };

    public static ITableStyle LightStyle2Accent6 => new TableStyle("Light Style 2 - Accent 6") { GUID = "{912C8C85-51F0-491E-9774-3900AFEF0FD7}" };

    public static ITableStyle LightStyle3 => new TableStyle("Light Style 3") { GUID = "{616DA210-FB5B-4158-B5E0-FEB733F419BA}" };

    public static ITableStyle LightStyle3Accent1 => new TableStyle("Light Style 3 - Accent 1") { GUID = "{BC89EF96-8CEA-46FF-86C4-4CE0E7609802}" };

    public static ITableStyle LightStyle3Accent2 => new TableStyle("Light Style 3 - Accent 2") { GUID = "{5DA37D80-6434-44D0-A028-1B22A696006F}" };

    public static ITableStyle LightStyle3Accent3 => new TableStyle("Light Style 3 - Accent 3") { GUID = "{8799B23B-EC83-4686-B30A-512413B5E67A}" };

    public static ITableStyle LightStyle3Accent4 => new TableStyle("Light Style 3 - Accent 4") { GUID = "{ED083AE6-46FA-4A59-8FB0-9F97EB10719F}" };

    public static ITableStyle LightStyle3Accent5 => new TableStyle("Light Style 3 - Accent 5") { GUID = "{BDBED569-4797-4DF1-A0F4-6AAB3CD982D8}" };

    public static ITableStyle LightStyle3Accent6 => new TableStyle("Light Style 3 - Accent 6") { GUID = "{E8B1032C-EA38-4F05-BA0D-38AFFFC7BED3}" };

    public static ITableStyle MediumStyle1 => new TableStyle("Medium Style 1") { GUID = "{793D81CF-94F2-401A-BA57-92F5A7B2D0C5}" };

    public static ITableStyle MediumStyle1Accent1 => new TableStyle("Medium Style 1 - Accent 1") { GUID = "{B301B821-A1FF-4177-AEE7-76D212191A09}" };

    public static ITableStyle MediumStyle1Accent2 => new TableStyle("Medium Style 1 - Accent 2") { GUID = "{9DCAF9ED-07DC-4A11-8D7F-57B35C25682E}" };

    public static ITableStyle MediumStyle1Accent3 => new TableStyle("Medium Style 1 - Accent 3") { GUID = "{1FECB4D8-DB02-4DC6-A0A2-4F2EBAE1DC90}" };

    public static ITableStyle MediumStyle1Accent4 => new TableStyle("Medium Style 1 - Accent 4") { GUID = "{1E171933-4619-4E11-9A3F-F7608DF75F80}" };

    public static ITableStyle MediumStyle1Accent5 => new TableStyle("Medium Style 1 - Accent 5") { GUID = "{FABFCF23-3B69-468F-B69F-88F6DE6A72F2}" };

    public static ITableStyle MediumStyle1Accent6 => new TableStyle("Medium Style 1 - Accent 6") { GUID = "{10A1B5D5-9B99-4C35-A422-299274C87663}" };

    public static ITableStyle MediumStyle2 => new TableStyle("Medium Style 2") { GUID = "{073A0DAA-6AF3-43AB-8588-CEC1D06C72B9}" };

    public static ITableStyle MediumStyle2Accent1 => new TableStyle("Medium Style 2 - Accent 1") { GUID = "{5C22544A-7EE6-4342-B048-85BDC9FD1C3A}" };

    public static ITableStyle MediumStyle2Accent2 => new TableStyle("Medium Style 2 - Accent 2") { GUID = "{21E4AEA4-8DFA-4A89-87EB-49C32662AFE0}" };

    public static ITableStyle MediumStyle2Accent3 => new TableStyle("Medium Style 2 - Accent 3") { GUID = "{F5AB1C69-6EDB-4FF4-983F-18BD219EF322}" };

    public static ITableStyle MediumStyle2Accent4 => new TableStyle("Medium Style 2 - Accent 4") { GUID = "{00A15C55-8517-42AA-B614-E9B94910E393}" };

    public static ITableStyle MediumStyle2Accent5 => new TableStyle("Medium Style 2 - Accent 5") { GUID = "{7DF18680-E054-41AD-8BC1-D1AEF772440D}" };

    public static ITableStyle MediumStyle2Accent6 => new TableStyle("Medium Style 2 - Accent 6") { GUID = "{93296810-A885-4BE3-A3E7-6D5BEEA58F35}" };

    public static ITableStyle MediumStyle3 => new TableStyle("Medium Style 3") { GUID = "{8EC20E35-A176-4012-BC5E-935CFFF8708E}" };

    public static ITableStyle MediumStyle3Accent1 => new TableStyle("Medium Style 3 - Accent 1") { GUID = "{6E25E649-3F16-4E02-A733-19D2CDBF48F0}" };

    public static ITableStyle MediumStyle3Accent2 => new TableStyle("Medium Style 3 - Accent 2") { GUID = "{85BE263C-DBD7-4A20-BB59-AAB30ACAA65A}" };

    public static ITableStyle MediumStyle3Accent3 => new TableStyle("Medium Style 3 - Accent 3") { GUID = "{EB344D84-9AFB-497E-A393-DC336BA19D2E}" };

    public static ITableStyle MediumStyle3Accent4 => new TableStyle("Medium Style 3 - Accent 4") { GUID = "{EB9631B5-78F2-41C9-869B-9F39066F8104}" };

    public static ITableStyle MediumStyle3Accent5 => new TableStyle("Medium Style 3 - Accent 5") { GUID = "{74C1A8A3-306A-4EB7-A6B1-4F7E0EB9C5D6}" };

    public static ITableStyle MediumStyle3Accent6 => new TableStyle("Medium Style 3 - Accent 6") { GUID = "{2A488322-F2BA-4B5B-9748-0D474271808F}" };

    public static ITableStyle MediumStyle4 => new TableStyle("Medium Style 4") { GUID = "{D7AC3CCA-C797-4891-BE02-D94E43425B78}" };

    public static ITableStyle MediumStyle4Accent1 => new TableStyle("Medium Style 4 - Accent 1") { GUID = "{69CF1AB2-1976-4502-BF36-3FF5EA218861}" };

    public static ITableStyle MediumStyle4Accent2 => new TableStyle("Medium Style 4 - Accent 2") { GUID = "{8A107856-5554-42FB-B03E-39F5DBC370BA}" };

    public static ITableStyle MediumStyle4Accent3 => new TableStyle("Medium Style 4 - Accent 3") { GUID = "{0505E3EF-67EA-436B-97B2-0124C06EBD24}" };

    public static ITableStyle MediumStyle4Accent4 => new TableStyle("Medium Style 4 - Accent 4") { GUID = "{C4B1156A-380E-4F78-BDF5-A606A8083BF9}" };

    public static ITableStyle MediumStyle4Accent5 => new TableStyle("Medium Style 4 - Accent 5") { GUID = "{22838BEF-8BB2-4498-84A7-C5851F593DF1}" };

    public static ITableStyle MediumStyle4Accent6 => new TableStyle("Medium Style 4 - Accent 6") { GUID = "{16D9F66E-5EB9-4882-86FB-DCBF35E3C3E4}" };

    public static ITableStyle DarkStyle1 => new TableStyle("Dark Style 1") { GUID = "{E8034E78-7F5D-4C2E-B375-FC64B27BC917}" };

    public static ITableStyle DarkStyle1Accent1 => new TableStyle("Dark Style 1 - Accent 1") { GUID = "{125E5076-3810-47DD-B79F-674D7AD40C01}" };

    public static ITableStyle DarkStyle1Accent2 => new TableStyle("Dark Style 1 - Accent 2") { GUID = "{37CE84F3-28C3-443E-9E96-99CF82512B78}" };

    public static ITableStyle DarkStyle1Accent3 => new TableStyle("Dark Style 1 - Accent 3") { GUID = "{D03447BB-5D67-496B-8E87-E561075AD55C}" };

    public static ITableStyle DarkStyle1Accent4 => new TableStyle("Dark Style 1 - Accent 4") { GUID = "{E929F9F4-4A8F-4326-A1B4-22849713DDAB}" };

    public static ITableStyle DarkStyle1Accent5 => new TableStyle("Dark Style 1 - Accent 5") { GUID = "{8FD4443E-F989-4FC4-A0C8-D5A2AF1F390B}" };

    public static ITableStyle DarkStyle1Accent6 => new TableStyle("Dark Style 1 - Accent 6") { GUID = "{AF606853-7671-496A-8E4F-DF71F8EC918B}" };

    public static ITableStyle DarkStyle2 => new TableStyle("Dark Style 2") { GUID = "{5202B0CA-FC54-4496-8BCA-5EF66A818D29}" };

    public static ITableStyle DarkStyle2Accent1Accent2 => new TableStyle("Dark Style 2 - Accent 1, Accent 2") { GUID = "{0660B408-B3CF-4A94-85FC-2B1E0A45F4A2}" };

    public static ITableStyle DarkStyle2Accent3Accent4 => new TableStyle("Dark Style 2 - Accent 3, Accent 4") { GUID = "{91EBBBCC-DAD2-459C-BE2E-F6DE35CF9A28}" };

    public static ITableStyle DarkStyle2Accent5Accent6 => new TableStyle("Dark Style 2 - Accent 5, Accent 6") { GUID = "{46F890A9-2807-4EBB-B81D-B2AA78EC7F39}" };

    public string Name { get; }

    public string GUID { get; private set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is ITableStyle style &&
               this.Name == style.Name &&
               this.GUID == style.GUID;
    }

    public override int GetHashCode()
    {
        int hashCode = 1242478914;
        hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.Name);
        hashCode = (hashCode * -1521134295) + EqualityComparer<string>.Default.GetHashCode(this.GUID);
        return hashCode;
    }
}
