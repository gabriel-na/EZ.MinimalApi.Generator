using System;

namespace EZ.MinimalApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class AllowAnonymousAttribute : Attribute
{}
