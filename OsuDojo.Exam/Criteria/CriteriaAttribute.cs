namespace OsuDojo.Exam.Criteria;

[AttributeUsage(AttributeTargets.Property)]
public class CriteriaAttribute(Type criteriaType) : Attribute
{
    public Type CriteriaType { get; } = criteriaType;
}
