public interface IMutationResult
{
    IMutationResultDescription Description { get; set; }
}

public interface IMutationResultDescription;

internal sealed class MutationSuccessfulResult;

internal sealed class MutationErrorResult;

internal sealed class MutationFailureResult;
