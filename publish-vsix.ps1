$vsixpublish = Get-ChildItem -File .\packages -recurse | 
    Where-Object { $_.Name -eq "VsixPublisher.exe" } | 
    Sort-Object -Descending -Property CreationTime | 
    Select-Object -First 1 -ExpandProperty FullName

#发布账号登陆
. $vsixpublish login -publisherName netcorevip -personalAccessToken $env:commenttranslatorchinapublish

#相关描述文件，vsix文件路径
$overview = (Get-Item .\overview.md).FullName
$manifest = (Get-Item .\publish-manifest.json).FullName
$vsix = (Get-Item .\CommentTranslator\bin\Release\CommentTranslatorChina.vsix).FullName
Write-Host "vsix: $vsix"
Write-Host "manifest: $manifest"
Write-Host "overview: $overview"

# . $vsixpublish deleteExtension -extensionName "HomeSeerTemplates" -publisherName "netcorevip"

. $vsixpublish publish -payload "$vsix" -publishManifest "$manifest"