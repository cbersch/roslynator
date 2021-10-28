﻿// Copyright (c) Josef Pihrt and Contributors. Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Host;
using Microsoft.CodeAnalysis.VisualBasic;

namespace Roslynator.VisualBasic
{
    [Export(typeof(ILanguageService))]
    [ExportMetadata("Language", LanguageNames.VisualBasic)]
    [ExportMetadata("ServiceType", "Roslynator.ISyntaxFactsService")]
    internal sealed class VisualBasicSyntaxFactsService : ISyntaxFactsService
    {
        public static VisualBasicSyntaxFactsService Instance { get; } = new VisualBasicSyntaxFactsService();

        public string SingleLineCommentStart => "'";

        public bool IsEndOfLineTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.EndOfLineTrivia);
        }

        public bool IsComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.CommentTrivia);
        }

        public bool IsSingleLineComment(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.CommentTrivia);
        }

        public bool IsWhitespaceTrivia(SyntaxTrivia trivia)
        {
            return trivia.IsKind(SyntaxKind.WhitespaceTrivia);
        }

        public SyntaxTriviaList ParseLeadingTrivia(string text, int offset = 0)
        {
            return SyntaxFactory.ParseLeadingTrivia(text, offset);
        }

        public SyntaxTriviaList ParseTrailingTrivia(string text, int offset = 0)
        {
            return SyntaxFactory.ParseTrailingTrivia(text, offset);
        }

        public bool BeginsWithAutoGeneratedComment(SyntaxNode root)
        {
            return GeneratedCodeUtility.BeginsWithAutoGeneratedComment(
                root,
                f => f.IsKind(SyntaxKind.CommentTrivia));
        }

        public bool AreEquivalent(SyntaxTree oldTree, SyntaxTree newTree)
        {
            return SyntaxFactory.AreEquivalent(oldTree, newTree, topLevel: false);
        }

        public SyntaxNode GetSymbolDeclaration(SyntaxToken identifier) => throw new NotImplementedException();

        public bool IsValidIdentifier(string name)
        {
            return SyntaxFacts.IsValidIdentifier(name);
        }
    }
}
