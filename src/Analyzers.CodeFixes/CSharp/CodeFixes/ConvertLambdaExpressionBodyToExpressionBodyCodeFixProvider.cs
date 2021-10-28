﻿// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Composition;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Roslynator.CodeFixes;
using Roslynator.CSharp.Refactorings;

namespace Roslynator.CSharp.CodeFixes
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ConvertLambdaExpressionBodyToExpressionBodyCodeFixProvider))]
    [Shared]
    public sealed class ConvertLambdaExpressionBodyToExpressionBodyCodeFixProvider : BaseCodeFixProvider
    {
        public override ImmutableArray<string> FixableDiagnosticIds
        {
            get { return ImmutableArray.Create(DiagnosticIdentifiers.ConvertLambdaExpressionBodyToExpressionBody); }
        }

        public override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            SyntaxNode root = await context.GetSyntaxRootAsync().ConfigureAwait(false);

            if (!TryFindFirstAncestorOrSelf(root, context.Span, out BlockSyntax block))
                return;

            CodeAction codeAction = CodeAction.Create(
                ConvertLambdaExpressionBodyToExpressionBodyRefactoring.Title,
                ct => ConvertLambdaExpressionBodyToExpressionBodyRefactoring.RefactorAsync(context.Document, (LambdaExpressionSyntax)block.Parent, ct),
                GetEquivalenceKey(DiagnosticIdentifiers.ConvertLambdaExpressionBodyToExpressionBody));

            context.RegisterCodeFix(codeAction, context.Diagnostics);
        }
    }
}
