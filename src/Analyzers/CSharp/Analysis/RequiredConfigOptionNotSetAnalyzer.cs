﻿// Copyright (c) .NET Foundation and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Immutable;
using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Roslynator.CSharp.Analysis;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal sealed class RequiredConfigOptionNotSetAnalyzer : AbstractRequiredConfigOptionNotSetAnalyzer
{
    private static ImmutableArray<DiagnosticDescriptor> _supportedDiagnostics;

    private static readonly ConfigOptionDescriptor[] _useBlockBodyOrExpressionBodyOptions = [
        ConfigOptions.BodyStyle,
        ConfigOptions.ExpressionBodyStyleOnNextLine,
        ConfigOptions.UseBlockBodyWhenDeclarationSpansOverMultipleLines,
        ConfigOptions.UseBlockBodyWhenExpressionSpansOverMultipleLines,
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
    {
        get
        {
            if (_supportedDiagnostics.IsDefault)
                Immutable.InterlockedInitialize(ref _supportedDiagnostics, CommonDiagnosticRules.RequiredConfigOptionNotSet);

            return _supportedDiagnostics;
        }
    }

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterCompilationStartAction(compilationContext =>
        {
            var flags = Flags.None;

            CompilationOptions compilationOptions = compilationContext.Compilation.Options;

            compilationContext.RegisterSyntaxTreeAction(context =>
            {
                // files generated by source generator have relative path
                if (!Path.IsPathRooted(context.Tree.FilePath))
                    return;

                AnalyzerConfigOptions options = context.GetConfigOptions();

                Validate(ref context, compilationOptions, options, Flags.AddOrRemoveAccessibilityModifiers, ref flags, DiagnosticRules.AddOrRemoveAccessibilityModifiers, ConfigOptions.AccessibilityModifiers);
                Validate(ref context, compilationOptions, options, Flags.AddOrRemoveParenthesesFromConditionInConditionalOperator, ref flags, DiagnosticRules.AddOrRemoveParenthesesFromConditionInConditionalOperator, ConfigOptions.ConditionalOperatorConditionParenthesesStyle);
                Validate(ref context, compilationOptions, options, Flags.ConfigureAwait, ref flags, DiagnosticRules.ConfigureAwait, ConfigOptions.ConfigureAwait);
                Validate(ref context, compilationOptions, options, Flags.IncludeParenthesesWhenCreatingNewObject, ref flags, DiagnosticRules.IncludeParenthesesWhenCreatingNewObject, ConfigOptions.ObjectCreationParenthesesStyle);
                Validate(ref context, compilationOptions, options, Flags.NormalizeFormatOfEnumFlagValue, ref flags, DiagnosticRules.NormalizeFormatOfEnumFlagValue, ConfigOptions.EnumFlagValueStyle);
                Validate(ref context, compilationOptions, options, Flags.NormalizeNullCheck, ref flags, DiagnosticRules.NormalizeNullCheck, ConfigOptions.NullCheckStyle);
                Validate(ref context, compilationOptions, options, Flags.UseAnonymousFunctionOrMethodGroup, ref flags, DiagnosticRules.UseAnonymousFunctionOrMethodGroup, ConfigOptions.UseAnonymousFunctionOrMethodGroup);
                Validate(ref context, compilationOptions, options, Flags.UseBlockBodyOrExpressionBody, ref flags, DiagnosticRules.UseBlockBodyOrExpressionBody, _useBlockBodyOrExpressionBodyOptions);
                Validate(ref context, compilationOptions, options, Flags.UseEmptyStringLiteralOrStringEmpty, ref flags, DiagnosticRules.UseEmptyStringLiteralOrStringEmpty, ConfigOptions.EmptyStringStyle);
                Validate(ref context, compilationOptions, options, Flags.UseExplicitlyOrImplicitlyTypedArray, ref flags, DiagnosticRules.UseExplicitlyOrImplicitlyTypedArray, ConfigOptions.ArrayCreationTypeStyle);
                Validate(ref context, compilationOptions, options, Flags.UseHasFlagMethodOrBitwiseOperator, ref flags, DiagnosticRules.UseHasFlagMethodOrBitwiseOperator, ConfigOptions.EnumHasFlagStyle);
                Validate(ref context, compilationOptions, options, Flags.UseImplicitOrExplicitObjectCreation, ref flags, DiagnosticRules.UseImplicitOrExplicitObjectCreation, ConfigOptions.ObjectCreationTypeStyle);
                Validate(ref context, compilationOptions, options, Flags.NormalizeUsageOfInfiniteLoop, ref flags, DiagnosticRules.NormalizeUsageOfInfiniteLoop, ConfigOptions.InfiniteLoopStyle);
                Validate(ref context, compilationOptions, options, Flags.DocCommentSummaryStyle, ref flags, DiagnosticRules.FormatDocumentationCommentSummary, ConfigOptions.DocCommentSummaryStyle);
            });
        });
    }

    private static void Validate(
        ref SyntaxTreeAnalysisContext context,
        CompilationOptions compilationOptions,
        AnalyzerConfigOptions configOptions,
        Flags flag,
        ref Flags flags,
        DiagnosticDescriptor analyzer,
        ConfigOptionDescriptor option)
    {
        if (!flags.HasFlag(flag)
            && analyzer.IsEffective(context.Tree, compilationOptions, context.CancellationToken)
            && TryReportRequiredOptionNotSet(context, configOptions, analyzer, option))
        {
            flags |= flag;
        }
    }

    private static void Validate(
        ref SyntaxTreeAnalysisContext context,
        CompilationOptions compilationOptions,
        AnalyzerConfigOptions configOptions,
        Flags flag,
        ref Flags flags,
        DiagnosticDescriptor analyzer,
        params ConfigOptionDescriptor[] options)
    {
        if (!flags.HasFlag(flag)
            && analyzer.IsEffective(context.Tree, compilationOptions, context.CancellationToken)
            && TryReportRequiredOptionNotSet(context, configOptions, analyzer, options))
        {
            flags |= flag;
        }
    }

    [Flags]
    private enum Flags
    {
        None,
        AddOrRemoveAccessibilityModifiers,
        AddOrRemoveParenthesesFromConditionInConditionalOperator,
        ConfigureAwait,
        DocCommentSummaryStyle,
        IncludeParenthesesWhenCreatingNewObject,
        NormalizeFormatOfEnumFlagValue,
        NormalizeNullCheck,
        NormalizeUsageOfInfiniteLoop,
        UseAnonymousFunctionOrMethodGroup,
        UseBlockBodyOrExpressionBody,
        UseEmptyStringLiteralOrStringEmpty,
        UseExplicitlyOrImplicitlyTypedArray,
        UseHasFlagMethodOrBitwiseOperator,
        UseImplicitOrExplicitObjectCreation,
    }
}
