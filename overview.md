# Comment Translator China

[![Build status](https://dev.azure.com/netcorevip/BuildsGit/_apis/build/status/comment-translator-china-CI)](https://dev.azure.com/netcorevip/BuildsGit/_build/latest?definitionId=9)[![Build status](https://camo.githubusercontent.com/378681ed7a5fb7af01aeb954371b93a75ce42f1a/68747470733a2f2f63692e6170707665796f722e636f6d2f6170692f70726f6a656374732f7374617475732f6a3264783675386c707777693735676e3f7376673d74727565)](https://ci.appveyor.com/project/corevip/comment-translator-china)

------

选中文本翻译，未选中默认翻译光标所在行，在打开文件中自动翻译注释。

因为[Comment Translator](https://marketplace.visualstudio.com/items?itemName=NguynThunTan.CommentTranslator)扩展在中国(你懂滴原因)无法访问配置的翻译地址，

现修改为使用<https://translate.google.cn/>翻译。

支持vs版本：Visual Studio 2017以上，Visual Studio 2017以下未测试。

### 初始化设置

需要设置谷歌TKK值才能使用翻译功能，下一步骤自动设置

[[![设置](https://github.com/corevip/comment-translator-china/raw/master/Images/Options.png)](https://github.com/corevip/comment-translator-china/blob/master/Images/Options.png)

### 自动设置TKK

> 如果获取失败打开浏览器手动获取tkk设置

[![自动设置tkk](https://github.com/corevip/comment-translator-china/raw/master/Images/SetTkk.gif)](https://github.com/corevip/comment-translator-china/blob/master/Images/SetTkk.gif)

## 功能

#### 手动翻译

- 选中文本通过右键单击翻译菜单或快捷键Alt+Z翻译
- 如果未选择文本，则自动选择行
- 一键复制翻译的文本

[![手动翻译](https://github.com/corevip/comment-translator-china/raw/master/Images/Auto-Selecte-Line.gif)](https://github.com/corevip/comment-translator-china/blob/master/Images/Auto-Selecte-Line.gif)

#### 自动翻译

- 自动翻译打开文件中的注释。

##### 支持语言文件

- CSharp
- C/C++
- FSharp
- Visual Basic
- Html
- CSS
- JavaScript/TypeScript
- Xml
- Xaml
- Python
- Razor

##### 打开/关闭打开文件自动翻译功能

- 右键单击打开/关闭自动翻译或者快捷键ALT+Shift+Z
- 工具->选项->Comment Translator China->里面设置打开/关闭

[![SetAuto](https://github.com/corevip/comment-translator-china/raw/master/Images/SetAuto.gif)](https://github.com/corevip/comment-translator-china/blob/master/Images/SetAuto.gif)

#### 在打开文件中自动翻译

> 已打开文件，关闭文件重新打开生效

[![自动翻译注释](https://github.com/corevip/comment-translator-china/raw/master/Images/Auto-File-translate.gif)](https://github.com/corevip/comment-translator-china/blob/master/Images/Auto-File-translate.gif)

来源：基于扩展[Comment Translator](https://marketplace.visualstudio.com/items?itemName=NguynThunTan.CommentTranslator)修改

## 贡献

如果您想为此项目做出贡献，请查看[贡献指南](https://github.com/corevip/comment-translator-china/blob/master/CONTRIBUTING.md)。

## License

[Apache 2.0](https://github.com/corevip/comment-translator-china/blob/master/LICENSE.txt)