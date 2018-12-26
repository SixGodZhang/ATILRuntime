# ATILRuntime

ATILRuntime 是在Unity编译器下进行ILRuntime开发的一组自动化工具.

## 主要功能:
- [适配器生成](#适配器生成)
- [自动绑定](#自动绑定)
- [热更工程单元测试](#热更工程单元测试)
- [主工程Bug修复](#主工程Bug修复)

## 测试环境
Unity 2018.2.17f1 .NET Runtime4.X

## 适配器生成
应用场景:通常应用于自定义类型适配器的生成
注意:
- 需要自动生成适配器的类型需要被[NeedAdaptor]标记
- 适配器只会继承和实现抽象、虚方法
- 白名单中的接口和类型不需要被标记，会自动添加到生成名单中
- 热更工程中使用的各种Action、Func、主工程中的delegate的实例可以自动分析，然后注册
- 若需要为委托自动生成转换器，请用[DelegateExport]特性进行标记

有一种情况需要注意,若在一个自定义类型中定义了其它类型/接口的引用,会在同一个文件下生成适配器(需要优化)

![委托生成器](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/delegate.png)

## 自动绑定
采用的自动分析功能来实现的自动绑定
![绑定生成器](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/binding.png)

## 热更工程单元测试
热更工程中的public static修饰的方法可以进行单元测试(目前只实现了public static)
![单元测试](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/unitTest.png)

## 主工程Bug修复
在热更工程中可以修复主工程的功能.
思想:
1.主工程中被[AllowHotfix]特性标记的都允许在热更工程中进行修复
2.修复的主要原理是:在主工程中增加一个委托实例,该实例的参数签名与我们要修复的方法的参数签名一致，然后在修复的方法判断该委托实例有没有被注册，若有则执行注册的方法
否则，则执行原有的逻辑(委托实例的生成是由工具自动完成的，我们只需要在允许修复的类型上面标记[AllowHotfix]特性即可，其它由工具自动完成)
![inject](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/inject.png)
3.在热更工程中为主工程中需要修复的委托实例注册方法:
![注册修复的方法](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/register.png)

![功能](https://github.com/SixGodZhang/ATILRuntime/blob/master/Images/function.png)

## 结尾
测试用例还比较少，有些内容不够完善，之后我会慢慢完善这些内容,若有任何疑问，欢迎大家提 issue