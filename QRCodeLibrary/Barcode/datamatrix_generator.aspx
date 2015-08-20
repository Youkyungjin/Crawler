<%@ Page Language="C#" %>
<%@ Import Namespace="BusinessRefinery.Barcode.Web" %>
<%
    HttpDataMatrix.drawBarcode(Request, Response);
%>
