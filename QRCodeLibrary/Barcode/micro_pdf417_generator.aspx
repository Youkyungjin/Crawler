<%@ Page Language="C#" %>
<%@ Import Namespace="BusinessRefinery.Barcode.Web" %>
<%
    HttpMicroPDF417.drawBarcode(Request, Response);
%>

