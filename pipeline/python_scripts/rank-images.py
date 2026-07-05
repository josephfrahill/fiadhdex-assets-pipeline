import os
from google import genai
from google.genai import types
from PIL import Image
from pydantic import BaseModel, Field

# Initialize the client (automatically uses GEMINI_API_KEY environment variable)
client = genai.Client()

# 1. Define the Structured Output Schema
class AnimalEvaluation(BaseModel):
    overall_score: int = Field(
        ..., 
        description="A score from 1 to 10 evaluating the overall photo representation of the animal."
    )
    subject_clarity: int = Field(
        ..., 
        description="Score from 1 to 10 indicating how clearly the animal is visible and in-focus."
    )
    posture_score: int = Field(
        ..., 
        description="Score from 1 to 10 representing how well the animal's posture shows its features."
    )
    reasoning: str = Field(
        ..., 
        description="A brief explanation justifying the assigned scores."
    )

# 2. Load the animal image
# Make sure to provide the path to your actual image file
image_path = "path/to/your/animal_image.jpg"
try:
    img = Image.open(image_path)
except FileNotFoundError:
    print(f"Error: The file {image_path} was not found.")
    exit(1)

# 3. Create the prompt and rubric
prompt = """
You are an expert wildlife and domestic animal evaluator. 
Analyze the provided image of the animal and score it using the following rubric:
- Subject Clarity (1-10): Is the animal sharp, in focus, and distinct from the background?
- Posture (1-10): Is the animal positioned in a way that clearly displays its physical structure and features?
- Overall Score (1-10): An overall evaluation of the image's quality as a reference photo of the animal.

Provide your output strictly matching the requested JSON structure.
"""

# 4. Call the Gemini API with the image and schema configuration
response = client.models.generate_content(
    model="gemini-2.5-flash",
    contents=[img, prompt],
    config=types.GenerateContentConfig(
        response_mime_type="application/json",
        response_schema=AnimalEvaluation,
    ),
)

# 5. Access the parsed, type-safe data
evaluation: AnimalEvaluation = response.parsed

print(f"Overall Score: {evaluation.overall_score}/10")
print(f"Subject Clarity: {evaluation.subject_clarity}/10")
print(f"Posture Score: {evaluation.posture_score}/10")
print(f"Reasoning: {evaluation.reasoning}")