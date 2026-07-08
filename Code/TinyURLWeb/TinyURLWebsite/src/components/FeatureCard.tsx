interface FeatureCardProps {
  image: string;
  title: string;
  text: string;
}

const FeatureCard = ({ image, title, text }: FeatureCardProps) => {
  return (
    <div className="group relative overflow-hidden rounded-3xl border border-gray-200 bg-white p-8 shadow-sm transition-all duration-300 hover:-translate-y-3 hover:shadow-2xl">

      {/* Background Glow */}
      <div className="absolute inset-0 bg-gradient-to-br from-blue-50 via-white to-white opacity-0 group-hover:opacity-100 transition duration-500" />

      <div className="relative z-10">

        {/* Image */}
        <div className="flex justify-center mb-6">
          <div className="bg-blue-50 rounded-2xl p-4 group-hover:bg-blue-100 transition duration-300">

            <img
              src={image}
              alt={title}
              className="w-24 h-24 object-contain group-hover:scale-110 transition-transform duration-300"
            />

          </div>
        </div>

        {/* Title */}
        <h3 className="text-xl font-bold text-gray-900 text-center mb-4">
          {title}
        </h3>

        {/* Description */}
        <p className="text-gray-600 text-center leading-7 mb-6">
          {text}
        </p>

        {/* Divider */}
        <div className="w-12 h-1 bg-blue-600 rounded-full mx-auto mb-6 group-hover:w-20 transition-all duration-300"></div>

       
        

      </div>

    </div>
  );
};

export default FeatureCard;